using React_ASPNETCORE_JWT_Auth.DataAccess.Entities;

namespace React_ASPNETCORE_JWT_Auth.Models
{
    public class UserAuthModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string EmailId { get; set; }
        public Role Role { get; set; }

    }
}
