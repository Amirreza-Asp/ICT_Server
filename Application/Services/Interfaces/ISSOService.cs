using Domain.Dtos.Account.SSO;
using Domain.Entities.Account;

namespace Application.Services.Interfaces
{
    public interface ISSOService
    {
        Task<ProfileRequest> GetProfileAsync(string token);

        Task<OAuthResponseToken> GetTokenAsync(string code);

        Task<List<Company>> GetCompaniesAsync();
    }
    public class CompsRequestData
    {
        public string id { get; set; }
        public string nameUniversity { get; set; }
        public string logoUniversity { get; set; }
        public string portalUrl { get; set; }
        public object singleWindowUrl { get; set; }
        public object nationalSerial { get; set; }
        public object latitude { get; set; }
        public object longitude { get; set; }
        public string cityName { get; set; }
        public string provinceName { get; set; }
        public string universityType { get; set; }
        public string status { get; set; }
        public string createAt { get; set; }
    }
}
