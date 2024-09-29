public class SiparisFisi
{
    public string BelgeNo { get; set; }
    public string CariKod { get; set; }
    public DateTime Tarih { get; set; }
    public decimal Iskonto { get; set; }
    public decimal ToplamTutar { get; set; }
    public List<SiparisKalemi> Kalemler { get; set; }
}

public class SiparisKalemi
{
    public string MalzemeKodu { get; set; }
    public decimal Miktar { get; set; }
    public string Birim { get; set; }
    public decimal Fiyat { get; set; }
    public decimal KDVOrani { get; set; }
}