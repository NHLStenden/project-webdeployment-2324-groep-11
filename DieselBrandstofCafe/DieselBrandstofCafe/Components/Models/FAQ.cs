namespace DieselBrandstofCafe.Components.Models
{
    public class FAQ
    {
        public int FAQID { get; set; }
        public required string FAQVraag { get; set; }
        public required string FAQAntwoord { get; set; }
    }
}
