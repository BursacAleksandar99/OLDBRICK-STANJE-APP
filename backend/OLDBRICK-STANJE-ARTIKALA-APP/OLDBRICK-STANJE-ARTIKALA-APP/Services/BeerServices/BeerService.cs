using Microsoft.EntityFrameworkCore;
using OLDBRICK_STANJE_ARTIKALA_APP.Data;
using OLDBRICK_STANJE_ARTIKALA_APP.DTOs.Beers;
using OLDBRICK_STANJE_ARTIKALA_APP.Entities;

namespace OLDBRICK_STANJE_ARTIKALA_APP.Services.BeerServices
{
    public class BeerService : IBeerService
    {
        private readonly AppDbContext _context;
        private readonly IProsutoService _prosutoService;

        public BeerService(AppDbContext context, IProsutoService prosutoService)
        {
            _context = context;
            _prosutoService = prosutoService;
        }

        public async Task<Beer> CreateAsync(CreateBeerRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.NazivPiva))
                throw new ArgumentException("Naziv piva je obavezan");

            if (string.IsNullOrWhiteSpace(request.TipMerenja))
                throw new ArgumentException("Tip merenja je obavezan.");

            var beer = new Beer
            {
                NazivPiva = request.NazivPiva.Trim(),
                TipMerenja = request.TipMerenja.Trim()
            };

            _context.Beers.Add(beer);
            await _context.SaveChangesAsync();

            return beer;
        }


        public async Task<Beer?> GetByIdAsync(int id)
        {
            return await _context.Beers.FindAsync(id);
        }

        public async Task<List<Beer>> GetAllBeersAsync()
        {
            return await _context.Beers.OrderBy(b => b.NazivPiva).ToListAsync();
        }

        public async Task SaveDailyBeerShortageAsync(int idNaloga)
        {
            if (idNaloga <= 0)
                throw new ArgumentException("IdNaloga nije validan.");

            // 1) Ne dozvoljavamo dupli upis za isti nalog
            var alreadyExists = await _context.DailyBeerShortages
                .AnyAsync(x => x.IdNaloga == idNaloga);

            if (alreadyExists)
                throw new InvalidOperationException("Manjak po pivu je vec upisan za ovaj nalog.");

            // 2) Ucitaj nalog zbog datuma (isti izvor kao u GetAllStatesByIdNaloga)
            var report = await _context.DailyReports
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.IdNaloga == idNaloga);

            if (report == null)
                throw new ArgumentException("Dnevni nalog ne postoji.");

            // Ako je Datum DateOnly u DailyReports:
            var datum = DateTime.SpecifyKind(
                        new DateTime(report.Datum.Year, report.Datum.Month, report.Datum.Day),
                        DateTimeKind.Utc
                    );

            // 3) Uzmi DTO koji vec racuna "odstupanje" po pivu
            var dto = await _prosutoService.GetAllStatesByIdNaloga(idNaloga);

            if (dto?.Items == null || dto.Items.Count == 0)
                throw new ArgumentException("Nema stavki za upis manjka.");

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var rows = dto.Items.Select(i => new DailyBeerShortage
                {
                    IdNaloga = idNaloga,
                    Datum = datum,
                    IdPiva = i.IdPiva,
                    Manjak = (float)Math.Round(i.Odstupanje, 2, MidpointRounding.AwayFromZero),// <-- OVO JE  "MANJAK"
                    CreatedAt = DateTime.UtcNow
                }).ToList();

                await _context.DailyBeerShortages.AddRangeAsync(rows);
                await _context.SaveChangesAsync();

                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<List<BeerShortageSumDto>> GetBeerShortageTotalsSinceLastInventoryAsync(int idNaloga)
        {
            if (idNaloga <= 0)
                throw new ArgumentException("IdNaloga nije validan.");

            // 1) Datum naloga (DateOnly)
            var report = await _context.DailyReports
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.IdNaloga == idNaloga);

            if (report == null)
                throw new ArgumentException("Dnevni nalog ne postoji.");

            var reportDate = report.Datum; // DateOnly

            // 2) Nadji poslednji popis na ili pre reportDate
            var reportStartUtc = new DateTime(
                                 reportDate.Year, reportDate.Month, reportDate.Day,
                                 0, 0, 0, DateTimeKind.Utc
                             );

            var lastReset = await _context.InventoryResets
                .AsNoTracking()
                .Where(x => x.DatumPopisa < reportStartUtc) // KLJUC: popis pre ovog dana (ne isti dan)
                .OrderByDescending(x => x.DatumPopisa)
                .ThenByDescending(x => x.Id)
                .FirstOrDefaultAsync();

            // 3) Odredi fromDate = dan posle popisa; ako nema popisa -> kreni od "najranijeg moguceg"
            DateOnly fromDate;
            if (lastReset != null)
            {
                var resetDay = DateOnly.FromDateTime(lastReset.DatumPopisa);
                fromDate = resetDay.AddDays(1); // KLJUC: od sledeceg dana
            }
            else
            {
                // nema popisa: sabiraj sve do reportDate
                fromDate = DateOnly.MinValue;
            }

            // Ako je popis "danas" ili posle danas (od sledeceg dana), nema sta da se sabira
            if (fromDate > reportDate)
                return new List<BeerShortageSumDto>();

            // 4) Prevedi DateOnly -> UTC midnight range za query
            var fromUtc = DateTime.SpecifyKind(
                new DateTime(fromDate.Year, fromDate.Month, fromDate.Day, 0, 0, 0),
                DateTimeKind.Utc
            );

            var toUtcExclusive = DateTime.SpecifyKind(
                new DateTime(reportDate.Year, reportDate.Month, reportDate.Day, 0, 0, 0),
                DateTimeKind.Utc
            ).AddDays(1);

            // 5) SUM(manjak) po pivu u opsegu [fromUtc, toUtcExclusive)
            var result = await _context.DailyBeerShortages
                .AsNoTracking()
                .Where(x => x.Datum >= fromUtc && x.Datum < toUtcExclusive)
                .GroupBy(x => x.IdPiva)
                .Select(g => new
                {
                    IdPiva = g.Key,
                    Total = g.Sum(x => x.Manjak)
                })
                .Join(_context.Beers.AsNoTracking(),
                    x => x.IdPiva,
                    b => b.Id,
                    (x, b) => new BeerShortageSumDto
                    {
                        IdPiva = x.IdPiva,
                        NazivPiva = b.NazivPiva,
                        TotalManjak = MathF.Round(x.Total, 2)
                    })
                .OrderBy(x => x.NazivPiva)
                .ToListAsync();

            return result;
        }

    }
}
