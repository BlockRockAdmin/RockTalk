using Supabase.Postgrest.Models;

namespace RockTalk
{
    public class Message : BaseModel
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? RoomId { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Extension { get; set; }
    }
}
