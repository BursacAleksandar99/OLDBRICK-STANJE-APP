namespace OLDBRICK_STANJE_ARTIKALA_APP.DTOs.Beers
{
    public class UpdateDailyBeerStateDto
    {
        public int IdPiva { get; set; }
        public float? Izmereno { get; set; }
        public float? StanjeUProgramu { get; set; }
    }
}
