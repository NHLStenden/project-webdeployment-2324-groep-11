namespace DieselBrandstofCafe.Components.Models
{
    public class Bestelling
    {
        public int BestellingID { get; set; }
        public int TafelID { get; set; }
        public required string Status { get; set; }
        public DateTime Tijd { get; set; }
        public required List<Product> Producten { get; set; }
        public int Kostenplaatsnummer { get; set; }
        public int TotaalPrijs { get; set; }
        public int BestelrondeID { get; set; }
    }
}
