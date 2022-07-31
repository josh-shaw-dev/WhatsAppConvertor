using System;

namespace WhatsAppConvertor.Domain.Dto
{
    public class ChatMessageDto 
    {
        public int MessageSortId { get; set; }

        public bool MessageFromMe { get; set; }

        public DateTime MessageRecievedTime { get; set; }

        public string? MessageText { get; set; }

        public string? FilePath { get; set; }

        public string? Thumbnail { get; set; }

        public MessageType? MessageType { get; set; }
    }
}
