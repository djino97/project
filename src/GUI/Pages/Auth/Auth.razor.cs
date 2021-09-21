using LT.DigitalOffice.GUI.Services.ApiClients.AuthService;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace LT.DigitalOffice.GUI.Pages.Auth
{
    public partial class Auth
    {
        private AuthenticationRequest _authData = new AuthenticationRequest();
        private string _message;

        private ElementReference _passwordInput;

        private async Task HandleValidSubmitAsync()
        {
            try
            {
                var loginResponse = await authService.LoginAsync(_authData);

                await authService.SetLoginStateAsync(
                    loginResponse.UserId,
                    loginResponse.AccessToken,
                    loginResponse.RefreshToken);

                UriHelper.NavigateTo("");
            }
            catch (ApiException<ErrorResponse> ex)
            {
                _message = ex.Result.Message;
            }
        }
    }
}
