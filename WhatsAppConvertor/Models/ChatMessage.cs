namespace WhatsAppConvertor.Models
{
    public class ChatMessage
    {
        public string? RawStringJid { get; set; }

        public int MessageId { get; set; }

        public int MessageSortId { get; set; }

        public bool MessageFromMe { get; set; }

        public long MessageRecievedTime { get; set; }

        public string? MessageText { get; set; }

        public string? MediaFilePath { get; set; }

        public int ChatId { get; set; }
    }

}
