namespace ContactManagerApp.Models
{
    public class SelectContactViewModel
    {
        public int ContactId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool IsSelected { get; set; }
    }
}
