namespace React_ASPNETCORE_JWT_Auth.DataAccess.Entities
{
    public class BaseModel
    {
        public Guid Id { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ModificationDate { get; set; }  
    }
}
