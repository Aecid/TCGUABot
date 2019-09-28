using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using TCGUABot.Data;
using Telegram.Bot.Types.InputFiles;

namespace TCGUABot
{
    public class TimeServiceGetSpoilers : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private Timer _timer;

        private readonly IServiceScopeFactory scopeFactory;

        public TimeServiceGetSpoilers(ILogger<TimeServiceGetSpoilers> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            this.scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is starting.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromHours(1));

            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var results = Helpers.MythicSpoilerParsing.GetNewSpoilers(dbContext);
                var client = await Bot.Get();
                var chats = dbContext.TelegramChats.Where(tc => tc.SendSpoilers == true).ToList();
                //for testing purposes
                //results.Add(new Data.Models.MythicSpoiler() { Url = "http://fossfolks.com/wp-content/uploads/images/news/small-business-calendar-software-testing.jpg" });
                if (results.Count <= 5)
                {
                    foreach (var result in results)
                    {
                        try
                        {
                            var req = WebRequest.Create(result.Url);

                            using (Stream fileStream = req.GetResponse().GetResponseStream())
                            {
                                var data = ReadFully(fileStream);

                                foreach (var chat in chats)
                                {
                                    using (Stream stream = new MemoryStream(data))
                                    {

                                        var caption = result.Url.Replace(".jpg", ".html");
                                        try
                                        {
                                            await client.SendPhotoAsync(chat.Id, new InputOnlineFile(stream), caption: caption, Telegram.Bot.Types.Enums.ParseMode.Html);
                                            await Task.Delay(300);
                                        }
                                        catch { }
                                    }
                                }
                            }
                        }
                        catch
                        {

                        }
                    }
                }
                else
                {
                    foreach (var chat in chats)
                    {
                        var msg = "Lots of new spoilers at http://www.mythicspoiler.com/newspoilers.html";

                        await client.SendTextMessageAsync(chat.Id, msg, Telegram.Bot.Types.Enums.ParseMode.Html);

                        await Task.Delay(300);

                    }
                }
            }
        }

        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public static DateTime GetLocalTime()
        {
            try
            {
                TimeZoneInfo helsinkiTZ = TimeZoneInfo.FindSystemTimeZoneById("Europe/Helsinki");
                DateTime localTime = TimeZoneInfo.ConvertTime(DateTime.Now, helsinkiTZ);

                return localTime;
            }
            catch
            {
                TimeZoneInfo helsinkiTZ = TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time");
                DateTime localTime = TimeZoneInfo.ConvertTime(DateTime.Now, helsinkiTZ);

                return localTime;
            }
        }
    }
}
