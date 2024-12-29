using System;
using System.ComponentModel.DataAnnotations;

namespace aspnetapp
{
    public class Account
    {
        [Key] public string userId { get; set; } = null!;
        public string openId { get; set; } = null!;
        public string token { get; set; } = null!;
        public DateTime loginTime { get; set; }
    }

    public class GameData
    {
        [Key] public string userId { get; set; } = null!;
        public byte[] data { get; set; } = null!;
    }
}