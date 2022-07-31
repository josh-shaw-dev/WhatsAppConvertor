using System.Collections.Generic;
using WhatsAppConvertor.Domain.Dto;

namespace RazorTemplates.Models
{
    public class ChatGroupModel
    {
        public IEnumerable<ChatGroupDto> ChatGroups { get; set; }
    }
}
