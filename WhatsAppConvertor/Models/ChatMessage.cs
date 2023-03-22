using WhatsAppConvertor.Domain;

namespace WhatsAppConvertor.Models
{
    public class ChatMessage
    {
        public string? RawStringJid { get; set; }

        public int MessageId { get; set; }

        public int MessageSortId { get; set; }

        public bool MessageFromMe { get; set; }

        public long Timestamp { get; set; }

        public string? MessageText { get; set; }
        
        public string? QuotedText { get; set; }

        public string? FilePath { get; set; }

        public byte[]? Thumbnail { get; set; }

        public MessageType? MessageType { get; set; }

        public int ChatId { get; set; }
    }

}
