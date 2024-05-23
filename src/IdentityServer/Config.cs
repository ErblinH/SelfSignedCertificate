using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Security.Claims;

namespace IdentityServer;

public class Config
{
    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            new Client
            {
                ClientId = "certificate_mvc_client",
                ClientName = "Certificate MVC Web",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                //RequirePkce = false,
                AllowRememberConsent = false,
                RedirectUris = new List<string>()
                {
                    "https://localhost:7277/signin-oidc"
                },
                PostLogoutRedirectUris = new List<string>()
                {
                    "https://localhost:7277/signout-callback-oidc"
                },
                ClientSecrets = new List<Secret>()
                {
                    new Secret("secret".Sha256())
                },
                AllowedScopes = new List<string>()
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "certificateAPI"
                }
            }
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new ApiScope("certificateAPI","Certificate API")
        };

    public static IEnumerable<ApiResource> ApiResources => new ApiResource[] { };

    public static IEnumerable<IdentityResource> IdentityResources => new IdentityResource[]
    {
        new IdentityResources.OpenId(),
        new IdentityResources.Profile()
    };

    public static List<TestUser> TestUsers => new List<TestUser>
    {
        new TestUser()
        {
            SubjectId = "5BE86359-073C-434B-AD2D-A3932222DABE",
            Username = "erblin",
            Password = "erblin",
            Claims = new List<Claim>()
            {
                new Claim(JwtClaimTypes.GivenName, "erblin"),
                new Claim(JwtClaimTypes.FamilyName, "halabaku")
            }
        }
    };
}