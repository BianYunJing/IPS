using IPS.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace IPS.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticateService _authService;
        public AuthenticationController(IAuthenticateService authService)
        {
            this._authService = authService;
        }

        [AllowAnonymous]
        [HttpPost, Route("requestToken")]
        public ActionResult RequestToken([FromBody] LoginRequestDTO request)
        {
            authRes ar = new authRes();
            if (ModelState.IsValid)
            {
                if (_authService.IsAuthenticated(request, out ar))
                {
                    return Ok(ar.resstr.ToString());
                }
                else
                    return BadRequest(JsonConvert.SerializeObject(ar.resstr.ToString()));
            }
            return BadRequest("{\"resType\":5,\"resStr\":\"Authentication Failed , Invalid Request!\"}");
        }
    }

    public class LoginRequestDTO
    {
        [Required]
        [JsonProperty("username")]
        public string Username { get; set; }


        [Required]
        [JsonProperty("password")]
        public string Password { get; set; }
    }

    public interface IAuthenticateService
    {
        bool IsAuthenticated(LoginRequestDTO request, out authRes token);
    }

    public class TokenAuthenticationService : IAuthenticateService
    {
        private readonly IUserService _userService;
        private readonly TokenManagement _tokenManagement;

        public TokenAuthenticationService(IUserService userService, IOptions<TokenManagement> tokenManagement)
        {
            _userService = userService;
            _tokenManagement = tokenManagement.Value;
        }
        public bool IsAuthenticated(LoginRequestDTO request, out authRes authres)
        {
            string token = string.Empty;
            authres = _userService.IsValid(request);
            if (!authres.res)
                return false;
            var claims = new[]
            {
                new Claim(ClaimTypes.Name,request.Username)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenManagement.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var jwtToken = new JwtSecurityToken(_tokenManagement.Issuer, _tokenManagement.Audience, claims, expires: DateTime.Now.AddMinutes(_tokenManagement.AccessExpiration), signingCredentials: credentials);

            try
            {
                token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
                authres.resstr = new StringBuilder("{\"resType\":1,\"resStr\":\"" + token + "\"}");
                //authres.resstr = new StringBuilder(@"{""resType"":1,""resStr"":""eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiQXBpQ2FsbGVyIiwiZXhwIjoxNjI3MzY4NTY0LCJpc3MiOiJ5YW5nemlqaWFuZy5jb20iLCJhdWQiOiJJUFNBUEkifQ._2nTWGdySsUG1wVpbJ1Hy9SfZGW0ZCDxsXV9iPhIOFM""}");
            }
            catch (Exception ex)
            {

            }

            return true;
        }
    }
    public interface IUserService
    {
        authRes IsValid(LoginRequestDTO req);
    }

    public class UserService : IUserService
    {
        public authRes IsValid(LoginRequestDTO req)
        {
            authRes authres = new authRes();
            authres.res = false;
            //默认账号密码，用于后端呼叫API时获取token使用
            if (req.Username.Equals("ApiCaller") && req.Password.Equals("Yzj123!"))
                authres.res = true;
            else
            {
                getToken gt = new getToken();
                authres = gt.GetTokenViaPostApi(req);
                //if ((bool)JsonConvert.SerializeObject(authres.resstr)[0])
            }
            return authres;
        }
    }

    public class authRes
    {
        public bool res { get; set; }

        public StringBuilder resstr { get; set; }
    }


    public class getToken
    {
        public authRes GetTokenViaPostApi(LoginRequestDTO loginDto)
        {
            authRes ar = new authRes();
            ar.res = false;
            Encoding encoding = Encoding.UTF8;
            string url = "https://localhost:44305/RoleManagement/Login";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST"; //Post请求方式
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.ContentType = "application/json"; //参数：username=admin&password=123 如果参数是json格式或者参数写错不会报错的

            byte[] buffer = encoding.GetBytes(JsonConvert.SerializeObject(loginDto).Replace("username", "RoleId"));
            request.ContentLength = buffer.Length;
            request.GetRequestStream().Write(buffer, 0, buffer.Length);

            //HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            //上面的代码会有异常出现，更改如下：

            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                response = (HttpWebResponse)ex.Response;
            }


            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                ar.resstr = new StringBuilder(reader.ReadToEnd());
                int res = Convert.ToInt32(ar.resstr.Replace("\"", "").ToString().Split(',')[0].Split(':')[1]);
                if (res > 0 && res < 4)
                    ar.res = true;
            }

            return ar;
        }
    }

}
