
using WhatsAppConvertor.Models;

namespace WhatsAppConvertor.Data
{
    public interface IMessageDataRepository
    {
        Task<IEnumerable<ChatMessage>> GetChats(CancellationToken cancellationToken = default);
    }
}