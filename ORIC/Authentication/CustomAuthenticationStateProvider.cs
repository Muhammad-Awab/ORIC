using ClassLibraryEnt;
using ORIC.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;

namespace ORIC.Authentication
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ProtectedSessionStorage? _sessionStorage;
        private ClaimsPrincipal? _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

        public CustomAuthenticationStateProvider(ProtectedSessionStorage sessionStorage)
        {
            _sessionStorage = sessionStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {

                var userSessionStorageResult = await _sessionStorage.GetAsync<UserSession>("UserSession");
                var userSession = userSessionStorageResult.Success ? userSessionStorageResult.Value : null;
                if (userSession == null)
                {
                    return await Task.FromResult(new AuthenticationState(_anonymous));
                }
                else
                {
                    var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                    { new Claim(ClaimTypes.Name, userSession.UserName.ToString()),
                      new Claim (ClaimTypes.Role ,userSession.Role),
                      new Claim ("UserId" ,userSession.UserId),

                    }, "CustomAuth"));
                    return await Task.FromResult(new AuthenticationState(claimsPrincipal));
                }




            }

            catch
            {
                return await Task.FromResult(new AuthenticationState(_anonymous));
            }
        }

        public async Task UpdateAuthenticationState(UserSession userregistration)
        {
            ClaimsPrincipal claimsPrincipal;

            if (userregistration != null)
            {
                await _sessionStorage.SetAsync("UserSession", userregistration);
                claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                    {
                      new Claim(ClaimTypes.Name, userregistration.UserName.ToString()),
                      new Claim (ClaimTypes.Role ,userregistration.Role),
                      new Claim ("UserId" ,userregistration.UserId),
                    }));
            }
            else
            {
                await _sessionStorage.DeleteAsync("UserSession");
                claimsPrincipal = _anonymous;
            }
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }


    }
}