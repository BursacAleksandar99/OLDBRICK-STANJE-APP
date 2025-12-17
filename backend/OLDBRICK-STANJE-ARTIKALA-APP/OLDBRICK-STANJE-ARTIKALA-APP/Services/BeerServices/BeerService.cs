using OLDBRICK_STANJE_ARTIKALA_APP.Data;
using OLDBRICK_STANJE_ARTIKALA_APP.DTOs.Beers;
using OLDBRICK_STANJE_ARTIKALA_APP.Entities;

namespace OLDBRICK_STANJE_ARTIKALA_APP.Services.BeerServices
{
    public class BeerService : IBeerService
    {
        private readonly AppDbContext _context;

        public BeerService(AppDbContext context)
        {
            _context = context;
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
    }
}
