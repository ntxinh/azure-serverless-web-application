using Core.Interfaces;

namespace Core.Entities
{
    public class Sample : BaseEntityAudit, IAggregateRoot
    {
        private Sample()
        {
            // required by EF
        }

        public Sample(string email, string password)
        {
            Email = email;
            Password = password;
        }

        public string Email { get; set; } = string.Empty;
        public string Password { get; set; }
    }
}
