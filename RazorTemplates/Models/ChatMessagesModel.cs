using System.Collections.Generic;
using System.Linq;
using WhatsAppConvertor.Domain.Dto;

namespace RazorTemplates.Models
{
    public class ChatMessagesModel
    {
        public IEnumerable<ChatMessageDto> ChatMessages { get; set; }

        public IEnumerable<ContactDto> Contacts { get; set; }

        public IEnumerable<ChatMessageAndContactDto> ChatMessagesAndContacts { get; set; }

    }
}
