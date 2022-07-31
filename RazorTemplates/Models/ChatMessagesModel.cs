using System.Collections.Generic;
using WhatsAppConvertor.Domain.Dto;

namespace RazorTemplates.Models
{
    public class ChatMessagesModel
    {
        public string DisplayName { get; set; }

        public IEnumerable<ChatMessageDto> ChatMessages { get; set; }
    }
}
