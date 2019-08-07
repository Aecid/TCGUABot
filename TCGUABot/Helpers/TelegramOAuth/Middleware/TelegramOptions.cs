using TCGUABot.Helpers.TelegramOAuth.Widget;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;

namespace TCGUABot.Helpers.TelegramOAuth.Middleware
{
    public class TelegramOptions : OAuthOptions
    {

        public PathString LoginPath { get; }
        public ButtonStyle ButtonStyle { get; set; }
        public bool ShowUserPhoto { get; set; }
        public bool RequestAccess { get; set; }
        public string BotUsername { get; set; }
        private string _generatedRedirectCode = null;
        public string GeneratedRedirectCode => _generatedRedirectCode ??
                                               (_generatedRedirectCode = WidgetEmbedCodeGenerator.GenerateRedirectEmbedCode(BotUsername, CallbackPath, ButtonStyle, ShowUserPhoto, RequestAccess));

        public TelegramOptions()
        {
            CallbackPath = new PathString("/signin-telegram");
            LoginPath = new PathString("/login-telegram");
            ButtonStyle = ButtonStyle.Large;
            ShowUserPhoto = true;
            RequestAccess = true;
            AuthorizationEndpoint = "/login-telegram";
            TokenEndpoint = "/login-telegram";
            UserInformationEndpoint = "/login-telegram";

            ClaimActions.MapJsonKey("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", "id",
                "http://www.w3.org/2001/XMLSchema#uinteger64");

            ClaimActions.MapJsonKey("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", "username",
                "http://www.w3.org/2001/XMLSchema#string");
        }
    }
}