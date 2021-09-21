
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Components;
using Blazored.SessionStorage;
using LT.DigitalOffice.GUI.Pages.Auth;
using LT.DigitalOffice.GUI.Services.ApiClients.AuthService;
using LT.DigitalOffice.GUI.Services.Interfaces;
using LT.DigitalOffice.GUI.Helpers;
using Microsoft.AspNetCore.Components.Authorization;
using LT.DigitalOffice.GUI.Models;

namespace LT.DigitalOffice.GUI.Services
{
    public class AuthService : IAuthService
    {
        private readonly AuthServiceClient _authService;
        private readonly AuthStateProvider _provider;
        private readonly ISessionStorageService _storage;
        private readonly NavigationManager _uriHelper;

        private async Task SetTokenValues(string accessToken, string refreshToken)
        {
            var handler = new JwtSecurityTokenHandler();

            Func<string, string> tokenExpiresIn = token => ((handler.ReadToken(token)) as JwtSecurityToken)
                .Claims.First(x => string.Equals("exp", x.Type)).Value;
            
            TokensValues.AccessToken = accessToken;     
            TokensValues.AccessTokenExpiresIn = tokenExpiresIn(accessToken);
            TokensValues.RefreshTokenExpiresIn = tokenExpiresIn(refreshToken);

            await _storage.SetItemAsync(Consts.AccessToken, accessToken);
            await _storage.SetItemAsync(Consts.RefreshToken, refreshToken);
            await _storage.SetItemAsync(Consts.AccessTokenExpiresIn, TokensValues.AccessTokenExpiresIn);
            await _storage.SetItemAsync(Consts.RefreshTokenExpiresIn, TokensValues.RefreshTokenExpiresIn);
        }

        public AuthService(AuthenticationStateProvider provider, ISessionStorageService storage, NavigationManager uriHelper)
        {
            _authService = new AuthServiceClient(new HttpClient());
            _provider = provider as AuthStateProvider;
            _storage = storage;
            _uriHelper = uriHelper;
        }

        public async Task<AuthenticationResponse> LoginAsync(AuthenticationRequest request)
        {
            return await _authService.LoginAsync(request);
        }

        public async Task SetLoginStateAsync(Guid userId, string accessToken, string refreshToken)
        {
            _provider.LoginNotify(userId);
            
            await SetTokenValues(accessToken, refreshToken);
            await _storage.SetItemAsync(nameof(Consts.UserId), userId);
        }

        public async Task RefreshTokenAsync()
        {
            RefreshRequest body = new()
            {
                RefreshToken = await _storage.GetItemAsync<string>(Consts.RefreshToken)
            };

            var refreshTokenData = await _authService.RefreshAsync(body);

            await SetTokenValues(refreshTokenData.AccessToken, refreshTokenData.RefreshToken);
        }

        public async Task<bool> LogoutAsync()
        {
            _provider.LogoutNotify();

            await _storage.ClearAsync();

            _uriHelper.NavigateTo("/login");

            return true;
        }

        public async Task<bool> AuthorizeAsync()
        {
            if (!await _storage.ContainKeyAsync(Consts.UserId))
            {
                return false;
            }

            TokensValues.AccessTokenExpiresIn = await _storage.GetItemAsync<string>(Consts.AccessTokenExpiresIn);
            TokensValues.RefreshTokenExpiresIn = await _storage.GetItemAsync<string>(Consts.RefreshTokenExpiresIn);

            _provider.LoginNotify(await _storage.GetItemAsync<Guid>(Consts.UserId));
            _uriHelper.NavigateTo(await _storage.GetItemAsync<string>(Consts.PageUri));

            return true;
        }
    }
}
