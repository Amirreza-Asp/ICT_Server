using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Application.Services.Interfaces
{
    public interface ITokenValidate
    {
        Task Execute(TokenValidatedContext context);
    }

}
