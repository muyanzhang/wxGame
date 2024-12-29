using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class LoginRequest {
    public string openId { get; set; }
}
public class LoginResponse {
    public string token { get; set; }
}

namespace aspnetapp.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly GameContext _context;

        public AuthController(GameContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login(LoginRequest req)
        {
            // 模拟微信登录逻辑 (生产环境需调用微信接口获取 openId)
            if (string.IsNullOrWhiteSpace(req.openId))
                return BadRequest("Invalid OpenId");

            // 查找用户
            var user = await _context.Users.FirstOrDefaultAsync(u => u.OpenId == req.openId);

            if (user == null)
            {
                // 如果用户不存在，则创建新用户
                user = new User
                {
                    OpenId = req.openId,
                    Token = Guid.NewGuid().ToString(),
                    LastActiveAt = DateTime.Now
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }
            else
            {
                // 如果用户已存在，则更新 Token 和活动时间
                user.Token = Guid.NewGuid().ToString();
                user.LastActiveAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }

            return new LoginResponse { token = user.Token };
        }

        [HttpPost("validate")]
        public async Task<ActionResult> ValidateToken([FromHeader] string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return Unauthorized();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Token == token);
            if (user == null)
                return Unauthorized();

            user.LastActiveAt = DateTime.Now;
            await _context.SaveChangesAsync();

            return Ok();
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

        private async Task<User?> GetAuthenticatedUser(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            return await _context.Users.FirstOrDefaultAsync(u => u.Token == token);
        }

        [HttpGet("fetch")]
        public async Task<ActionResult<List<GameData>>> FetchData([FromHeader] string token)
        {
            var user = await GetAuthenticatedUser(token);
            if (user == null)
                return Unauthorized();

            var data = await _context.GameData.Where(d => d.UserId == user.OpenId).ToListAsync();
            return Ok(data);
        }

        [HttpPost("upload")]
        public async Task<ActionResult> UploadData([FromHeader] string token, [FromBody] Dictionary<string, string> data)
        {
            var user = await GetAuthenticatedUser(token);
            if (user == null)
                return Unauthorized();

            foreach (var entry in data)
            {
                var existingData = await _context.GameData
                    .FirstOrDefaultAsync(d => d.UserId == user.OpenId);

                if (existingData != null)
                {
                    // 更新数据
                    existingData.Value = entry.Value;
                }
                else
                {
                    // 插入新数据
                    _context.GameData.Add(new GameData
                    {
                        UserId = user.OpenId,
                        Value = entry.Value
                    });
                }
            }

            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
