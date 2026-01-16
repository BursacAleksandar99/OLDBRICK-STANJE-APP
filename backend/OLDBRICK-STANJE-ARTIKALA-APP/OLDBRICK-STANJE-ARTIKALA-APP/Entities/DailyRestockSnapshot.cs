namespace OLDBRICK_STANJE_ARTIKALA_APP.Entities
{
    public class DailyRestockSnapshot
    {
        public long Id { get; set; }

        public int IdNaloga { get; set; }

        public int IdPiva { get; set; }

        public float AddedQuantity { get; set; }

        public float IzmerenoSnapshot { get; set; }

        public float PosSnapshot { get; set; }

        public DateOnly SourceDate { get; set; }

        public int SourceIdNaloga { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
