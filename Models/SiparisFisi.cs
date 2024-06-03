// Models/SiparisFisi.cs
namespace OzzeJobTrackerRestApi.Models
{
    public class SiparisFisi
    {
        public string BelgeNo { get; set; }
        public string CariKod { get; set; }
        public DateTime Tarih { get; set; }
        public float Iskonto { get; set; }
        public float IskontoTutari { get; set; }
        public float DovizKuru { get; set; }
        public List<SiparisKalemi> Kalemler { get; set; }
    }

    public class SiparisKalemi
    {
        public string MalzemeKodu { get; set; }
        public float Miktar { get; set; }
        public string Birim { get; set; }
        public float Fiyat { get; set; }
        public float Toplam { get; set; }
        public float IskontoTutari { get; set; }
        public float VergiMatrahı { get; set; }
        public float ToplamNet { get; set; }
        public float DovizKuru { get; set; }
    }
}
