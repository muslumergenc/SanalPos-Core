using IsbankCoreMvc.Models;
using IsBankMvc.Abstraction.Interfaces.Payments;
using IsBankMvc.Abstraction.Models.Payments;
using IsBankMvc.Abstraction.Types;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace IsbankCoreMvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPaymentService _service;
        public HomeController(ILogger<HomeController> logger, IPaymentService service)
        {
            _logger = logger;
            _service = service;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(PreparePaymentRequestBase obj)
        {
            if (!ModelState.IsValid)
                return View(OperationResult<PreparePaymentResponse>.Rejected("model validation"));
            obj.RequestIp = GetIP();
            obj.OrderId = Guid.NewGuid();
            obj.Language = "TR";
            var result = await _service.PreparePayment(obj);
            if (result.Data is null)
            {
                return View(OperationResult<PreparePaymentResponse>.Rejected("Ödeme hazýrlýðý baþarýsýz oldu"));
            }
            return Content(result.Data.Markup, "text/html");
        }
        public IActionResult Success()
        {
            // Ödeme baþarýlý olduðunda gösterilecek sayfa
            return View();
        }

        protected string GetIP()
        {
            var ip = HttpContext.Request.Headers["CF-Connecting-IP"].ToString();
            if (string.IsNullOrEmpty(ip))
                ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;

            if (string.IsNullOrEmpty(ip)) ip = "127.0.0.1";

            return ip;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
