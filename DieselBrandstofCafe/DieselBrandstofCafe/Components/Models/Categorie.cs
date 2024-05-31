namespace DieselBrandstofCafe.Components.Models
{
    public class Categorie
    {
        public int CategoryID { get; set; }
        public required string NaamCategorie { get; set; }
        public int? ParentID { get; set; }
    }
}
