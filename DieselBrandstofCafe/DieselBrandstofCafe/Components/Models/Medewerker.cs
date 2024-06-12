namespace DieselBrandstofCafe.Components.Models
{
    public class Medewerker
    {
        public int MedewerkerID { get; set; }
        public required string MedewerkerNaam { get; set; }
        public required string EmailMedewerker { get; set; }
        public required string Telefoonnummer { get; set; }
        public required string MedewerkerInlognaam { get; set; }
        public required string MedewerkerWachtwoord { get; set; }
        public required string MedewerkerType { get; set; }
        public int? SupervisorID { get; set; }  // Foreign key to the supervisor
        public required string MedewerkerStatus { get; set; }
        public DateTime MedewerkerGeboortedatum { get; set; }
        public required string MedewerkerAdres { get; set; }
        public required string MedewerkerPostcode { get; set; }
        public required string MedewerkerWoonplaats { get; set; }
        public required string MedewerkerRol { get; set; }
        public int MedewerkerContracturen { get; set; }
        public required string MedewerkerContracttype { get; set; }
        public int MedewerkerSalaris { get; set; }
        public required string MedewerkerBankrekening { get; set; }
        public DateTime MedewerkerContractBegin { get; set; }
        public DateTime MedewerkerContractEinde { get; set; }
        public string? MedewerkerAfbeelding { get; set; }



        // Navigation property for the supervisor
        public Medewerker? Supervisor { get; set; }

        // Navigation property for the employees supervised by this employee
        /*public List<Empolyee> Subordinates { get; set; }*/
    }
}

