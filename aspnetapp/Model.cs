using System;

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
        public int Id { get; set; }
        public string OpenId { get; set; } = null!;
        public string Token { get; set; } = null!;
        public DateTime LastActiveAt { get; set; }
    }

    public class GameData
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Key { get; set; } = null!;
        public string Value { get; set; } = null!;
    }

}