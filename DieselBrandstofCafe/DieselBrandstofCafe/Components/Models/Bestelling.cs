namespace DieselBrandstofCafe.Components.Models
{
    public class Bestelling
    {
        public int BestellingID { get; set; }
        public int TafelID { get; set; }
        public string? StatusBestelling { get; set; }
        public DateTime TijdBestelling { get; set; }
        public List<Product>? Producten { get; set; } = new List<Product>();
        public int Kostenplaatsnummer { get; set; }
        public decimal TotaalPrijs { get; set; }
        public int BestelrondeID { get; set; }
        public bool IsExpanded { get; set; } = false;
    }

}
