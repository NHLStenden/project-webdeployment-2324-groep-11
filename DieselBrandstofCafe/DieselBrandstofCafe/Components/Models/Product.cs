namespace DieselBrandstofCafe.Components.Models
{
    public class Product
    {
        public int ProductID { get; set; }
        public int CategoryID { get; set; }
        public string? ProductNaam { get; set; }
        public string? ProductDesc { get; set; }
        public decimal ProductPrijs { get; set; }
        public int Voorraad { get; set; }
        public string? Supplier { get; set; }
        public int? AddOnID { get; set; }
        public bool LactoseVrij { get; set; }
        public bool Vegetarisch { get; set; }
        public bool Veganistisch { get; set; }
        public string? ProductAfbeelding { get; set; }
        public int AantalBetaald { get; set; }

    }
}
