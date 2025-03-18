using System;
using System.Threading.Tasks;
using Autodesk.Authentication;
using Autodesk.Authentication.Model;

public partial class APS
{
    public string GetAuthorizationURL()
    {
        AuthenticationClient authenticationClient = new AuthenticationClient(_SDKManager);
        return authenticationClient.Authorize(_clientId, ResponseType.Code, _callbackUri, TokenScopes);

    }

    public async Task<Tokens> GenerateTokens(string code)
    {
        AuthenticationClient authenticationClient = new AuthenticationClient(_SDKManager);
        dynamic Token = await authenticationClient.GetThreeLeggedTokenAsync(_clientId, code, _callbackUri, _clientSecret);

        return new Tokens
        {
            AccessToken = Token.AccessToken,
            RefreshToken = Token.RefreshToken,
            ExpiresAt = DateTime.Now.ToUniversalTime().AddSeconds(Token.ExpiresIn)
        };
    }

    public async Task<Tokens> RefreshTokens(Tokens tokens)
    {
        AuthenticationClient authenticationClient = new AuthenticationClient(_SDKManager);
        dynamic Token = await authenticationClient.RefreshTokenAsync(tokens.RefreshToken, _clientId, _clientSecret, TokenScopes);
        return new Tokens
        {
            AccessToken = Token.AccessToken,
            RefreshToken = Token.RefreshToken,
            ExpiresAt = DateTime.Now.ToUniversalTime().AddSeconds(Token.ExpiresIn)
        };
    }

    public async Task<dynamic> GetUserProfile(Tokens tokens)
    {
        AuthenticationClient authenticationClient = new AuthenticationClient(_SDKManager);
        dynamic profile = await authenticationClient.GetUserInfoAsync(tokens.AccessToken);
        return profile;
    }
}