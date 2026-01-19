namespace OLDBRICK_STANJE_ARTIKALA_APP.Entities
{
    public class DailyBeerShortage
    {
        public long Id { get; set; }

        public int IdNaloga { get; set; }

        public DateTime Datum { get; set; }

        public int IdPiva { get; set; }

        public float Manjak { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
