namespace DieselBrandstofCafe.Components.Models
{
    public class ProductPerBestelronde
    {
        public int ProductID { get; set; }
        public int BestelrondeID { get; set; }
        public int AantalProduct { get; set; }
        public int AantalBetaald { get; set; }
        public string? StatusBesteldeProduct { get; set; }
        public DateTime VerkoopDatumProduct { get; set; }
        public string? ProductNaam { get; set; }
<<<<<<< HEAD
        public DateTime VerkoopDatumProduct { get; set; }
=======
        public int TotaleAantalProduct { get; set; }
        public int TotalProductsSold { get; set; }
        public string DayOfWeek { get; set; }
>>>>>>> b922b6dc8e6e02cb45a47db792681e94be414c84
    }
}
