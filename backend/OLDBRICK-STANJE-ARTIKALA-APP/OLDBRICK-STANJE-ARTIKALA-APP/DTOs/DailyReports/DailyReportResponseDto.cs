namespace OLDBRICK_STANJE_ARTIKALA_APP.DTOs.DailyReports
{
    public class DailyReportResponseDto
    {
        public int IdNaloga { get; set; }
        public DateOnly Datum { get; set; }
        public float TotalProsuto { get; set; }
        public float IzmerenoProsutoVaga { get; set; }
        public float IzracunataRazlikaProsutog {  get; set; }
    }
}
