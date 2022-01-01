namespace React_ASPNETCORE_JWT_Auth.Models
{
    public class BaseResponseModel
    {
        public bool IsSuccess { get; set; }
        public List<string> Errors { get; set; }
        public List<ErrorModel> UserFriendlyErrors { get; set; }
    }

    public class ErrorModel
    {
        public string Property { get; set; }
        public string Error { get; set; }

    }
}
