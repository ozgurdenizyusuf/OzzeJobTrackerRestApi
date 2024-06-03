// Models/AmbarFisi.cs
namespace OzzeJobTrackerRestApi.Models
{
    public class AmbarFisi
    {
        public string KaynakAmbarKodu { get; set; }
        public int KaynakAmbarNo { get; set; }
        public string HedefAmbarKodu { get; set; }
        public int HedefAmbarNo { get; set; }
        public string MalzemeKodu { get; set; }
        public float Miktar { get; set; }
        public string BelgeNo { get; set; }
        public string HareketOzelKodu2 { get; set; }
    }
}
