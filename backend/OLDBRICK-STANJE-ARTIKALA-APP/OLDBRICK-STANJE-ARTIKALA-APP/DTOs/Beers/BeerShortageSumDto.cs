namespace OLDBRICK_STANJE_ARTIKALA_APP.DTOs.Beers
{
    public class BeerShortageSumDto
    {
        public int IdPiva { get; set; }
        public string NazivPiva { get; set; } = "";
        public float TotalManjak { get; set; }
    }
}
