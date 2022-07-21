using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WhatsAppConvertor.Configuration;
using WhatsAppConvertor.Models;

namespace WhatsAppConvertor.Data
{
    public class ContactDataRepository : IContactDataRepository
    {
        private readonly ILogger<ContactDataRepository> _logger;
        private readonly WaDatabaseOptions _options;

        public ContactDataRepository(
            ILogger<ContactDataRepository> logger,
            IOptions<WaDatabaseOptions> options)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<IEnumerable<Contact>> GetContacts(CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Attempting to open connection to db at path {Path}", _options.ConnectionString);

            using SqliteConnection connection = new(_options.ConnectionString);
            await connection.OpenAsync(cancellationToken);

            SqliteCommand command = connection.CreateCommand();
            const string commandText =
            @"
                SELECT 
	                jid AS RawStringJid,
	                is_whatsapp_user as IsWhatsAppUser,
	                status as Status,
	                display_name as DisplayName,
	                given_name AS GivenName,
	                family_name AS FamilyName,
	                sort_name AS SortName
                FROM wa_contacts;
            ";

            CommandDefinition commandDefinition = new(commandText, cancellationToken: cancellationToken);

            IEnumerable<Contact> contacts = await connection.QueryAsync<Contact>(commandDefinition);

            await connection.CloseAsync();

            _logger.LogInformation("Retrieved {ContactCount} contacts from db", contacts.Count());



            return contacts;
        }
    }
}
