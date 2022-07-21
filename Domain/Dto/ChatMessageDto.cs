using System;

namespace WhatsAppConvertor.Domain.Dto
{
    public class ChatMessageDto
    {
        public int MessageId { get; set; }

        public int MessageSortId { get; set; }

        public string? MessageFrom { get; set; }

        public DateTime MessageRecievedTime { get; set; }

        public string? MessageText { get; set; }

        public int ChatId { get; set; }
    }

}
