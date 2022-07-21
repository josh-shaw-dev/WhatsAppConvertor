
using WhatsAppConvertor.Models;

namespace WhatsAppConvertor.Data
{
    public interface IContactDataRepository
    {
        Task<IEnumerable<Contact>> GetContacts(CancellationToken cancellationToken = default);
    }
}