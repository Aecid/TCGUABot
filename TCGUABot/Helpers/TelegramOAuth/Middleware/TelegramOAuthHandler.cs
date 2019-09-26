
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using TCGUABot.Helpers.TelegramOAuth.Widget;
using Authorization = TCGUABot.Helpers.TelegramOAuth.Widget.Authorization;

namespace TCGUABot.Helpers.TelegramOAuth.Middleware
{
    public class TelegramOAuthHandler : OAuthHandler<TelegramOptions>
    {
        public TelegramOAuthHandler(IOptionsMonitor<TelegramOptions> options, ILoggerFactory logger, UrlEncoder encoder,
            ISystemClock clock) : base(options, logger, encoder, clock) { }

        public override async Task<bool> ShouldHandleRequestAsync()
        {
            return Options.LoginPath == Request.Path || await base.ShouldHandleRequestAsync();
        }

        protected override string BuildChallengeUrl(AuthenticationProperties properties, string redirectUri)
        {

            Response.Cookies.Append("__Telegram", Options.StateDataFormat.Protect(properties), new CookieOptions() { IsEssential = true });
            return Options.AuthorizationEndpoint;
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            return base.HandleChallengeAsync(properties);
        }

        public override async Task<bool> HandleRequestAsync()
        {
            if (Options.LoginPath == Request.Path)
            {
                Response.Headers.Add("Content-Type", "text/html; charset=utf-8");
                await Response.WriteAsync("<html><head></head><body>" + Options.GeneratedRedirectCode + "</body></html>");
                //await Response.WriteAsync(@"<html><head></head><body><script async src=""https://telegram.org/js/telegram-widget.js?7"" data-telegram-login=""tcguabot"" data-size=""large"" data-auth-url=""https://ace.od.ua/signin-telegram"" data-request-access=""write""></script></body></html>");

                return true;
            }

            return await base.HandleRequestAsync();
        }



        protected override async Task<HandleRequestResult> HandleRemoteAuthenticateAsync()
        {
            TelegramOAuthHandler telegramOAuthHandler = this;

            LoginWidget loginWidget = new LoginWidget(telegramOAuthHandler.Options.ClientSecret);

            Dictionary<string, string> parameters =
                telegramOAuthHandler.Context.Request.Query.Keys.ToDictionary(k => k,
                    v => telegramOAuthHandler.Context.Request.Query[v].FirstOrDefault());

            if (Request.Query.ContainsKey("id") && Request.Query.ContainsKey("hash"))
            {
                var Items = new Dictionary<string, string>()
                {
                    { ".redirect", "/Identity/Account/ExternalLogin?returnUrl=%2F&handler=Callback" },
                    { "LoginProvider", "Telegram" }
                };
                var properties = new AuthenticationProperties(Items);

                try
                {
                    Request.Headers.Add("Cookie", "__Telegram = " + Options.StateDataFormat.Protect(properties));
                }
                catch { }
            }

            Authorization authorized = loginWidget.CheckAuthorization(parameters);

            if (authorized != Authorization.Valid)
            {
                return HandleRequestResult.Fail($"Authorization state: {authorized}");
            }

            TelegramUser telegramUser = new TelegramUser(parameters[Fields.Id]);

            ClaimsIdentity identity = new ClaimsIdentity(new[]
            {
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", telegramUser.Id,"http://www.w3.org/2001/XMLSchema#string", telegramOAuthHandler.ClaimsIssuer),
                //new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", telegramUser.Username,"http://www.w3.org/2001/XMLSchema#string", telegramOAuthHandler.ClaimsIssuer)
            }, telegramOAuthHandler.ClaimsIssuer);

            AuthenticationProperties authenticationProperties = null;

            var cookie = Request.Cookies["__Telegram"];

            if (string.IsNullOrEmpty(cookie) && !Request.Query.ContainsKey("hash"))
            {
                return HandleRequestResult.Fail("State cookie not present");
            }

            authenticationProperties = telegramOAuthHandler.Options.StateDataFormat.Unprotect(cookie);

            if (Request.Query.ContainsKey("id") && Request.Query.ContainsKey("hash"))
            {
                var ItemsA = new Dictionary<string, string>()
                {
                    { ".redirect", "/Identity/Account/ExternalLogin?returnUrl=%2F&handler=Callback" },
                    { "LoginProvider", "Telegram" }
                };

                authenticationProperties = new AuthenticationProperties(ItemsA);
            }

            if (authenticationProperties == null)
            {
                return HandleRequestResult.Fail("Authentication properties null");
            }

            JObject normalUser = JObject.FromObject(telegramUser);
            string json = normalUser.ToString(Newtonsoft.Json.Formatting.None);
            JsonDocument user = JsonDocument.Parse(json);

            Response.Cookies.Delete("__Telegram");
            return HandleRequestResult.Success(await telegramOAuthHandler.CreateTicketAsync(identity,
                authenticationProperties, OAuthTokenResponse.Success(user)));
        }


        private class TelegramUser
        {
            public string Id { get; }

            public TelegramUser(string id)
            {
                Id = id;
            }
        }

        private class Fields
        {
            public const string AuthDate = "auth_date";
            public const string FirstName = "first_name";
            public const string LastName = "last_name";
            public const string Id = "id";
            public const string PhotoUrl = "photo_url";
            public const string Username = "username";
            public const string Hash = "hash";
        }
    }
}