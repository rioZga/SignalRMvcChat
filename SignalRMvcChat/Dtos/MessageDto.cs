namespace SignalRMvcChat.Dtos
{
    public class MessageDto
    {
        public string text { get; set; }
        public string? username { get; set; }
        public int? conversationId { get; set; }
    }
}
