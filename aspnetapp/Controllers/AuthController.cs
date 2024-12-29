using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace aspnetapp.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly GameContext _context;

        private static string APP_ID = "wx4f4a03bbd5832f0e";
        private static string APP_SECRET = "866d21ab3b280f3b891121db18ee77fe";
        
        public class LoginRequest
        {
            public string code { get; set; }
        }

        public class LoginResponse
        {
            public string token { get; set; }
            public string data { get; set; }
        }

        public class WeChatResponse
        {
            public string OpenId { get; set; }
            public string SessionKey { get; set; }
            public string UnionId { get; set; }
        }

        public AuthController(GameContext context)
        {
            _context = context;
        }

        [HttpPost("wx/login")]
        public async Task<ActionResult<LoginResponse>> Login(LoginRequest req)
        {
            // 模拟微信登录逻辑 (生产环境需调用微信接口获取 openId)
            if (string.IsNullOrWhiteSpace(req.code))
                return BadRequest("Invalid code");

            try
            {
                // 使用微信的接口获取 openid
                // 假设你已经有了微信的 appId 和 appSecret
                var url =
                    $"https://api.weixin.qq.com/sns/jscode2session?appid={APP_ID}&secret={APP_SECRET}&js_code={req.code}&grant_type=authorization_code";

                var client = new HttpClient();
                var response = await client.GetStringAsync(url);

                // 解析返回的 JSON 获取 openid
                var responseData = JsonSerializer.Deserialize<WeChatResponse>(response);
                if(responseData == null || string.IsNullOrEmpty(responseData.OpenId))
                    return BadRequest("OpenId is Null");
                // 查找用户
                var user = await _context.Users.FirstOrDefaultAsync(u => u.openId == responseData.OpenId);
                var res = new LoginResponse();
                if (user == null)
                {
                    // 如果用户不存在，则创建新用户
                    user = new Account
                    {
                        openId = responseData.OpenId,
                        userId = Guid.NewGuid().ToString("N").Substring(0, 24),
                        token = Guid.NewGuid().ToString("N"),
                        loginTime = DateTime.Now
                    };
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    // 如果用户已存在，则更新 Token 和活动时间
                    user.token = Guid.NewGuid().ToString();
                    user.loginTime = DateTime.Now;
                    var data = await _context.GameData.Where(d => d.userId == user.userId).FirstOrDefaultAsync();
                    res.data = data.data;
                    await _context.SaveChangesAsync();
                }

                res.token = user.token;
                return res;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest("error");
            }
        }
    }
    
    
  

    [Route("api/data")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private readonly GameContext _context;

        public DataController(GameContext context)
        {
            _context = context;
        }

        private async Task<Account?> GetAuthenticatedUser(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            return await _context.Users.FirstOrDefaultAsync(u => u.token == token);
        }
        
        public class UploadRequest
        {
            public string token { get; set; }
            public string data { get; set; }
        }

        [HttpPost("upload")]
        public async Task<ActionResult> UploadData(UploadRequest req)
        {
            var user = await GetAuthenticatedUser(req.token);
            if (user == null)
                return Unauthorized();

            var gameData = await _context.GameData
                .FirstOrDefaultAsync(d => d.userId == user.userId);

            if (gameData != null)
            {
                // 更新数据
                gameData.data = req.data;
            }
            else
            {
                // 插入新数据
                _context.GameData.Add(new GameData
                {
                    userId = user.userId,
                    data = req.data
                });
            }

            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}