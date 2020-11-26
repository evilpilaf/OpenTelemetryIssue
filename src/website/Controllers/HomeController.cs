using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using otel_test_1.Models;
using StackExchange.Redis;

namespace otel_test_1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ConnectionMultiplexer _connection;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ConnectionMultiplexer connection, ILogger<HomeController> logger)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IActionResult> IndexAsync()
        {
            var db = _connection.GetDatabase();

            await db.StringSetAsync("test", "testvalue");

            string value = await db.StringGetAsync("test");

            _logger.LogInformation(value);

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
