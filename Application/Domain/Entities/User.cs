using Application.Common;

namespace Application.Domain.Entities
{
    public class User : AuditableEntity, IEntity<int>
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
    }
}
