using Microsoft.EntityFrameworkCore;
using OLDBRICK_STANJE_ARTIKALA_APP.Data;
using OLDBRICK_STANJE_ARTIKALA_APP.DTOs.Beers;
using OLDBRICK_STANJE_ARTIKALA_APP.DTOs.DailyReports;
using OLDBRICK_STANJE_ARTIKALA_APP.Entities;
using OLDBRICK_STANJE_ARTIKALA_APP.Services.DailyReports;

namespace OLDBRICK_STANJE_ARTIKALA_APP.Services.BeerServices
{
    public class DailyBeerStateService : IDailyBeerStateService
    {
        public readonly AppDbContext _context;
        public readonly IProsutoService _prosutoService;
        public readonly IDailyReportService _dailyReportService;

        public DailyBeerStateService(AppDbContext context, IProsutoService prosutoService,
            IDailyReportService dailyReportService)
        {
            _context = context;
            _prosutoService = prosutoService;
            _dailyReportService = dailyReportService;
        }

        public async Task<List<DailyBeerState>>UpsertForReportAsync(int idNaloga, List<UpsertDailyBeerStateDto> items)
        {
            if(items == null || items.Count == 0) 
                throw new ArgumentException("Lista svaki je prazna.");

            var reportExists = await _context.DailyReports.AnyAsync(x => x.IdNaloga == idNaloga);
            if(!reportExists)
                throw new ArgumentException("Dnevni nalog ne postoji.");

            var beerIds = items.Select(x => x.BeerId).Distinct().ToList();

            var beers = await _context.Beers.Where(b => beerIds.Contains(b.Id))
                .ToDictionaryAsync(b => b.Id, b => b.NazivPiva);

            var result = new List<DailyBeerState>();

            foreach( var dto in items)
            {
                if (dto.BeerId <= 0) throw new ArgumentException("BeerId nije validan.");
                if (dto.Izmereno < 0) throw new ArgumentException("Izmereno ne može biti negativno.");
                if (dto.StanjeUProgramu < 0) throw new ArgumentException("Stanje u programu ne može biti negativno.");
                if (!beers.TryGetValue(dto.BeerId, out var beerName))
                    throw new ArgumentException($"Pivo sa ID {dto.BeerId} ne postoji.");


                var existing = await _context.DailyBeerStates
                    .FirstOrDefaultAsync(x => x.IdNaloga == idNaloga && x.IdPiva == dto.BeerId);

                if(existing == null)
                {
                    var state = new DailyBeerState
                    {
                        IdNaloga = idNaloga,
                        IdPiva = dto.BeerId,
                        NazivPiva = beerName,
                        Izmereno = dto.Izmereno,
                        StanjeUProgramu = dto.StanjeUProgramu
                    };

                    _context.DailyBeerStates.Add(state);
                    result.Add(state);
                }
                else
                {
                    existing.NazivPiva = beerName;
                    existing.Izmereno = dto.Izmereno;
                    existing.StanjeUProgramu = dto.StanjeUProgramu;
                    result.Add(existing);
                }

               
            }
            await _context.SaveChangesAsync();
            return result;
        }

        public async Task<DailyBeerState?> AddQuantityAsync(int idNaloga, int idPiva, float kolicina)
        {
            if(idNaloga <= 0) throw new ArgumentException("IdNaloga nije validan.");
            if (idPiva <= 0) throw new ArgumentException("IdPiva nije validan.");
            if (kolicina <= 0) throw new ArgumentException("Kolicina mora biti veca od 0.");

            var state = await _context.DailyBeerStates
                .FirstOrDefaultAsync(x => x.IdNaloga == idNaloga && x.IdPiva == idPiva);

            if (state == null) return null;

            state.Izmereno += kolicina;
            state.StanjeUProgramu += kolicina;

            await _context.SaveChangesAsync();
            return state;
        }

        public async Task<List<DailyBeerState>> AddQuantityBatchAsync(int idNaloga, List<AddMoreBeerQuantityDto> items)
        {
            if(idNaloga <= 0) throw new ArgumentException("IdNaloga nije validan.");
            if (items == null || items.Count == 0) throw new ArgumentException("Items lista je prazna.");


            var grouped = items.
                GroupBy(x => x.IdPiva)
                .Select(g => new { IdPiva = g.Key, Kolicina = g.Sum(x => x.Kolicina) })
                .ToList();

         

            foreach(var x in grouped)
            {
                if (x.IdPiva <= 0) throw new ArgumentException("IdPiva nije validan.");
                if (x.Kolicina <= 0) throw new ArgumentException("Kolicina mora biti veca od 0.");
            }

            var idPivaList = grouped.Select(x => x.IdPiva).ToList();

            var tipMer = await _context.Beers.Where(b => idPivaList.Contains(b.Id))
                .ToDictionaryAsync(b => b.Id, b => b.TipMerenja);

            var states = await _context.DailyBeerStates
                .Where(s => s.IdNaloga == idNaloga && idPivaList.Contains(s.IdPiva))
                .ToListAsync();

            var foundIds = states.Select(s => s.IdPiva).ToHashSet();
            var missing = idPivaList.Where(id => !foundIds.Contains(id)).ToList();
            if(missing.Count > 0)
            {
                throw new KeyNotFoundException($"Ne postoji stanje u TAB3 za IdNaloga={idNaloga} za IdPiva: {string.Join(", ", missing)}");
            }

            using var tx = await _context.Database.BeginTransactionAsync();

            foreach(var state in states)
            {
                var add =grouped.First(x => x.IdPiva == state.IdPiva).Kolicina;

                _context.Restocks.Add(new Restock
                {
                    IdNaloga = idNaloga,
                    IdPiva = state.IdPiva,
                    Quantity = (decimal)add
                });

                var tip = tipMer[state.IdPiva]?.Trim().ToLowerInvariant();


               if(tip == "bure")
                {
                    state.Izmereno += add;
                    state.StanjeUProgramu += add;
                }
               else if(tip == "kesa")
                {
                    state.StanjeUProgramu += add;
                }
                else
                {
                    throw new ArgumentException("Nepoznat tip merenja za ovaj artikal.");
                }
            }

            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            return states;
        }

        public async Task DeleteReportAsync(int idNaloga)
        {
            if (idNaloga <= 0) throw new ArgumentException("IdNaloga nije validan.");

            var report = await _context.DailyReports
                .FirstOrDefaultAsync(x => x.IdNaloga == idNaloga);

            if (report == null)
                throw new KeyNotFoundException("Dnevni nalog ne postoji.");

            using var tx = await _context.Database.BeginTransactionAsync();

            try
            {
                var states = await _context.DailyBeerStates
                    .Where(s => s.IdNaloga == idNaloga)
                    .ToListAsync();
                _context.DailyBeerStates.RemoveRange(states);

                var restocks = await _context.Restocks
                    .Where(r => r.IdNaloga == idNaloga)
                    .ToListAsync();
                _context.Restocks.RemoveRange(restocks);

                _context.DailyReports.Remove(report);

                await _context.SaveChangesAsync();
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<ProsutoResultDto> UpdateStatesAndRecalculateAsync(int idNaloga, List<UpdateDailyBeerStateDto> items)
        {
            if (idNaloga <= 0) throw new ArgumentException("IdNaloga nije validan.");
            if (items == null || items.Count == 0) throw new ArgumentException("Items lista je prazna.");


            foreach(var dto in items)
            {
                if (dto.IdPiva <= 0) throw new ArgumentException("IdPiva nije validan.");
                if (dto.Izmereno is < 0) throw new ArgumentException("Izmereno ne sme biti negativno.");
                if (dto.StanjeUProgramu is < 0) throw new ArgumentException("Stanje u programu ne sme biti negativno.");
                if (dto.Izmereno == null && dto.StanjeUProgramu == null)
                    throw new ArgumentException("Moraš poslati bar jedno polje za update.");
            }

            var idPivaList = items.Select(x => x.IdPiva).Distinct().ToList();

            using var tx = await _context.Database.BeginTransactionAsync();

            var states = await _context.DailyBeerStates.Where(s => s.IdNaloga == idNaloga && idPivaList.Contains(s.IdPiva))
                .ToListAsync();

            var found = states.Select(s => s.IdPiva).ToHashSet();
            var missing = idPivaList.Where(id => !found.Contains(id)).ToList();
            if(missing.Count > 0)
            {
                throw new KeyNotFoundException
                    ($"Ne postoji stanje u TAB3 za IdNaloga={idNaloga} za IdPiva: " +
                    $"{string.Join(", ", missing)}");
            }

            foreach (var s in states)
            {
                var dto = items.First(x => x.IdPiva == s.IdPiva);

                if (dto.Izmereno.HasValue) s.Izmereno = dto.Izmereno.Value;
                if (dto.StanjeUProgramu.HasValue) s.StanjeUProgramu = dto.StanjeUProgramu.Value;
            }

            await _context.SaveChangesAsync();

            await _dailyReportService.RecalculateProsutoJednogPivaAsync(idNaloga);

            var result = await _prosutoService.CalculateAndSaveAsync(idNaloga);

            await tx.CommitAsync();
            return result;
        }



    }
}
