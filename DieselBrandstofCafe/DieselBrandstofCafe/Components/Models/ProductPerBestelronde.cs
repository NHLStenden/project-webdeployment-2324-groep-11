﻿namespace DieselBrandstofCafe.Components.Models
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
        public int TotaleAantalProduct { get; set; }
        public int TotalProductsSold { get; set; }
        public string? DayOfWeek { get; set; }

    }
}
