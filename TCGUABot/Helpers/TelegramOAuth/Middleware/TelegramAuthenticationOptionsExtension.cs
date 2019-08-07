using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.DependencyInjection;

namespace TCGUABot.Helpers.TelegramOAuth.Middleware
{
    public static class TelegramAuthenticationOptionsExtension
    {
        public static AuthenticationBuilder AddTelegram(this AuthenticationBuilder builder)
        {
            return builder.AddTelegram("Telegram", (Action<OAuthOptions>)(_ => { }));
        }

        public static AuthenticationBuilder AddTelegram(this AuthenticationBuilder builder,
            Action<TelegramOptions> configureOptions)
        {
            return builder.AddTelegram("Telegram", configureOptions);
        }

        public static AuthenticationBuilder AddTelegram(this AuthenticationBuilder builder, string authenticationScheme,
            Action<TelegramOptions> configureOptions)
        {
            return builder.AddTelegram(authenticationScheme, "Telegram", configureOptions);
        }

        public static AuthenticationBuilder AddTelegram(this AuthenticationBuilder builder, string authenticationScheme,
            string displayName, Action<TelegramOptions> configureOptions)
        {
            return builder.AddOAuth<TelegramOptions, TelegramOAuthHandler>(authenticationScheme, displayName,
                configureOptions);
        }
    }
}