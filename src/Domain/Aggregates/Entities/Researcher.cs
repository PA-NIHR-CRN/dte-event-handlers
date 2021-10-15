namespace Domain.Aggregates.Entities
{
    public class Researcher
    {
        public Researcher(string firstName, string lastname, string email)
        {
            Firstname = firstName;
            Lastname = lastname;
            Email = email;
        }
        
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
    }
}