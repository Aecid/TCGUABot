using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TCGUABot.Data;

namespace TCGUABot
{
    public class TimeService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private Timer _timer;
        private ApplicationDbContext _context;

        private readonly IServiceScopeFactory scopeFactory;

        public TimeService(ILogger<TimeService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            this.scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is starting.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(1));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {

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
