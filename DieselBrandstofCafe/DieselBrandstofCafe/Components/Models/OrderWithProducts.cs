namespace DieselBrandstofCafe.Components.Models
{
    // A helper class to hold orders and their associated products.
    public class OrderWithProducts
    {
        public int BestellingID { get; set; }
        public int TafelID { get; set; }
        public List<ProductPerBestelronde>? Products { get; set; }
        public string? StatusBestelling { get; set; }
        public int BestelrondeID { get; set; }
        public int? AantalProduct { get; set; }
        public DateTime CreationTime { get; set; }
        public TimeSpan ElapsedTime => DateTime.Now - CreationTime;


    }
}
