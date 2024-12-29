using System;
using System.ComponentModel.DataAnnotations;

namespace aspnetapp
{
    public class Counter
    {
        public int id { get; set; }
        public int count { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
    }
    
    public class User
    {
        [Key]
        public string OpenId { get; set; } = null!;
        public string Token { get; set; } = null!;
        public DateTime LastActiveAt { get; set; }
    }

    public class GameData
    {
        [Key]
        public string UserId { get; set; }
        public string Value { get; set; } = null!;
    }

}