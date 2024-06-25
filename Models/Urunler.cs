namespace OzzeJobTrackerRestApi.Models
{
    public class Urun
    {
        public string Kod { get; set; }
        public string Aciklama { get; set; }
        public string AnaBirim { get; set; }
        public decimal FiiliStok { get; set; }
        public string OzelKod { get; set; }
        public string GrupKodu { get; set; }
        public decimal? KoliIci { get; set; }
    }
}
