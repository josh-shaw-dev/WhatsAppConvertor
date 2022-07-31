using System.Collections.Generic;
using WhatsAppConvertor.Domain.Dto;

namespace RazorTemplates.Models
{
    public class ChatMessagesModel
    {
        public IEnumerable<ChatGroupDto> ChatGroups { get; set; }
    }
}
