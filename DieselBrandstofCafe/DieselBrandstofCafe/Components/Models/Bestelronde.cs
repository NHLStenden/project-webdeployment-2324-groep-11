namespace DieselBrandstofCafe.Components.Models
{
    public class Bestelronde
    {
        public int BestelrondeID { get; set; }
        public int OberID { get; set; }
        public int TafelID { get; set; }
        public required string Status { get; set; }
        public DateTime Tijd { get; set; }
    }
}
