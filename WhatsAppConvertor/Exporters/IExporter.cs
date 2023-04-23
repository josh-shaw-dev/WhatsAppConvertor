using WhatsAppConvertor.Models;

namespace WhatsAppConvertor.Exporters
{
    public interface IExporter
    {
        Task ExportAsync(IEnumerable<ChatMessage> chats, IEnumerable<Contact> contacts, IList<ChatMessageAndContact> messagesWithContacts);
    }
}