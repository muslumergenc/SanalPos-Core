using Azure.Core;
using IsbankCoreMvc.Models;
using IsbankCoreMvc.Services;
using IsBankMvc.Abstraction.Contracts;
using IsBankMvc.Abstraction.Enums;
using IsBankMvc.Abstraction.Models.User;
using IsBankMvc.Abstraction.Types;
using Microsoft.AspNetCore.Mvc;

namespace IsbankCoreMvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _service;
        private readonly ITokenService _tokenService;
        private readonly UserAuthData? _UserAuth;
        public AccountController(IUserService service, ITokenService tokenService, UserAuthData? UserAuth)
        {
            _service = service;
            _tokenService = tokenService;
            _UserAuth = UserAuth;
        }
        [HttpGet]
        public IActionResult Login(LoginRequest? request, string? text)
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var op = await _service.Login(request);
            if (op.Status != OperationResultStatus.Success)
                return View();
            var token = _tokenService.GenerateToken(op.Data!);
            var result = OperationResult<LoginResponse>
                .Success(new LoginResponse
                {
                    Id = op.Data.Id,
                    Token = token,
                    Email = op.Data.Username,
                    DisplayName = op.Data.Username,
                    UserType = op.Data.UserType
                });
            if (result is not null)
            {
                if (result.Data.UserType==UserType.Admin)
                {
                    return RedirectToAction("Index","Home");
                }
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Register(RegisterRequest? request,string text)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            if (_UserAuth!.UserType < UserType.Admin) 
                return (IActionResult)OperationResult<RegisterResponse>.Rejected();
            request.UserType = UserType.Admin;
            var op = await _service.Register(request, request.UserType);
            if (op.Status != OperationResultStatus.Success) return View();
            var token = _tokenService.GenerateToken(op.Data!);
            var result= OperationResult<RegisterResponse>.Success(new RegisterResponse
            {
                Id = op.Data.Id,
                Token = token,
                Email = op.Data.Username,
                Avatar = op.Data.Avatar,
                DisplayName = op.Data.Username,
                UserType = op.Data.UserType
            });

            return View();
        }
    }
}
