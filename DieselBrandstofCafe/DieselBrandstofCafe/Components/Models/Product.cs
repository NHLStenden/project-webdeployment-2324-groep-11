namespace DieselBrandstofCafe.Components.Models
{
    public class Product
    {
        public int ProductID { get; set; }
        public int CategoryID { get; set; }
        public required string ProductNaam { get; set; }
        public int Prijs { get; set; }
        public int Voorraad { get; set; }
        public int? AddOnID { get; set; }
    }
}
