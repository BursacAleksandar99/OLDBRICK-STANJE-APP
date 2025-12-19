using OLDBRICK_STANJE_ARTIKALA_APP.DTOs.Beers;

namespace OLDBRICK_STANJE_ARTIKALA_APP.DTOs.DailyReports
{
    public class ProsutoResultDto
    {
        public int IdNaloga { get; set; }
        public float TotalProsuto { get; set; } // L
        public List<BeerCalcResultDto> Items { get; set; } = new();

        //public float TotalPotrosenoVaga { get; set; } // L
        //public float TotalPotrosenoProgram { get; set; } // L
    }
}
