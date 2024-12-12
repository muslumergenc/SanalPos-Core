 
using IsBankMvc.Abstraction.Enums;
using IsBankMvc.Abstraction.Interfaces.Payments;
using IsBankMvc.Abstraction.Models.Payments;
using Microsoft.AspNetCore.Mvc;

namespace IsbankCoreMvc.Controllers.Webhooks
{
    public class IsBankWebHookController : Controller
    {
        private readonly IPaymentService _paymentService;

        public IsBankWebHookController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost]
        [Route("is-bank/success/{paymentId:guid}")]
        public async Task<IActionResult> IsBankSuccessCallback([FromRoute] Guid paymentId)
        {
             var returnUrl = await ProcessIsBankCallback(true, paymentId);
             return Redirect(returnUrl);
            // var response = await ProcessIsBankCallback(false, paymentId);
            // ViewBag["PaymentResult"] = response;
            // return Redirect(response);
        }
        [HttpPost]
        [Route("is-bank/fail/{paymentId:guid}")]
        public async Task<IActionResult> IsBankFailCallback([FromRoute] Guid paymentId)
        {
            var returnUrl = await ProcessIsBankCallback(true, paymentId);
            return Redirect(returnUrl);
            //var response = await ProcessIsBankCallback(false, paymentId);
            //ViewBag["PaymentResult"] = response;
           // return Redirect(response);
        }
        [HttpPost]
        [Route("is-bank/{paymentId:guid}")]
        public async Task<IActionResult> IsBankGenericCallback([FromRoute] Guid paymentId)
        {
            var returnUrl = await ProcessIsBankCallback(false, paymentId);
            return Redirect(returnUrl);
        }
        private async Task<string> ProcessIsBankCallback(bool success, Guid paymentId)
        {
            var response =_paymentService.BankCallback(new BankCallbackProcessRequest
            {
                Provider = ThirdPartyProvider.IsBank,
                Success = success,
                PaymentId = paymentId,
                Parameters = Request.Form.ToDictionary(
                    k => k.Key,
                    v => v.Value.ToString()
                )
            }).Result.ToString()!;
            return response;
        }
    }
}