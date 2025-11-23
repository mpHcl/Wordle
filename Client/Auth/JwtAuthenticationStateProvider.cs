
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using Blazored.LocalStorage;
using System.IdentityModel.Tokens.Jwt;

namespace Client.Auth {
    /// <summary>
    /// Provides authentication state information based on a stored JWT token.
    /// This implementation reads the token from browser local storage and
    /// constructs a <see cref="ClaimsPrincipal"/> based on its claims.
    /// </summary>
    public class JwtAuthenticationStateProvider : AuthenticationStateProvider {
        private readonly ILocalStorageService _localStorage;

        /// <summary>
        /// Initializes a new instance of the <see cref="JwtAuthenticationStateProvider"/> class.
        /// </summary>
        /// <param name="localStorage">The local storage service used to retrieve authentication data.</param>
        public JwtAuthenticationStateProvider(ILocalStorageService localStorage) {
            _localStorage = localStorage;
        }

        /// <summary>
        /// Retrieves the current authentication state of the user.
        /// Reads the JWT token from local storage, extracts claims,
        /// and constructs a <see cref="ClaimsPrincipal"/> accordingly.
        /// </summary>
        /// <returns>
        /// A <see cref="Task{AuthenticationState}"/> containing the user's authentication state.
        /// Returns an anonymous identity if no valid token is found.
        /// </returns>
        public override async Task<AuthenticationState> GetAuthenticationStateAsync() {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (string.IsNullOrWhiteSpace(token))
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var identity = new ClaimsIdentity(jwt.Claims, "jwt");

            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        /// <summary>
        /// Notifies the application that a user has successfully authenticated.
        /// This method parses the provided JWT token and updates the authentication state.
        /// </summary>
        /// <param name="token">The JWT token representing the authenticated user.</param>
        public void NotifyUserAuthentication(string token) {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var identity = new ClaimsIdentity(jwt.Claims, "jwt");
            var user = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        /// <summary>
        /// Notifies the application that the user has logged out.
        /// Resets the authentication state to an anonymous identity.
        /// </summary>
        public void NotifyUserLogout() {
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymousUser)));
        }
    }
}

