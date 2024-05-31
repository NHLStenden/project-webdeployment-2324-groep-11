namespace DieselBrandstofCafe.Components.Models
{
    public class Medewerker
    {
        public int MedewerkerID { get; set; }
        public required string MedewerkerNaam { get; set; }
        public int MedewerkerLeeftijd { get; set; } = 0;
        public required string MedewerkerEmail { get; set; }
        public required string MedewerkerTel { get; set; }
        public int? SupervisorID { get; set; }  // Foreign key to the supervisor


        // Navigation property for the supervisor
        public Medewerker? Supervisor { get; set; }

        // Navigation property for the employees supervised by this employee
        /*public List<Empolyee> Subordinates { get; set; }*/
    }
}

