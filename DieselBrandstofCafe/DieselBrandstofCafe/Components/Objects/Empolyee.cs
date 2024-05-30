namespace DieselBrandstofCafe.Components.Objects
{
    public class Empolyee
    {
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public int EmployeeAge { get; set; } = 0;
        public string EmployeeEmail { get; set; }
        public string EmployeePhone { get; set; }
        public int? SupervisorID { get; set; }  // Foreign key to the supervisor


        // Navigation property for the supervisor
        public Empolyee Supervisor { get; set; }

        // Navigation property for the employees supervised by this employee
        public List<Empolyee> Subordinates { get; set; }
    }
}
