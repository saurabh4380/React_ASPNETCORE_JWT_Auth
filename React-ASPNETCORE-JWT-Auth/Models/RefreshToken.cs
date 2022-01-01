namespace React_ASPNETCORE_JWT_Auth.Models
{
    public class RefreshToken
    {
        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public DateTime CreationTime { get; set; }
        public string CreatedByIp { get; set; }

    }
}
