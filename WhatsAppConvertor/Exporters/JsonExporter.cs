using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using WhatsAppConvertor.Configuration;
using WhatsAppConvertor.Models;

namespace WhatsAppConvertor.Exporters
{
    internal class JsonExporter : IExporter
    {
        private readonly ILogger<HtmlExporter> _logger;
        private readonly ExportOptions _options;

        public JsonExporter(
            ILogger<HtmlExporter> logger,
            IOptions<ExportOptions> options)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task ExportAsync(
            IEnumerable<ChatMessage> chats,
            IEnumerable<Contact> contacts,
            IList<ChatMessageAndContact> messagesWithContacts)
        {
            if (_options.Json)
            {
                string messageOutputPath = Path.Combine(_options.Directory, "messages.json");
                string contactsOutputPath = Path.Combine(_options.Directory, "contacts.json");

                _logger.LogInformation("Text export enabled, exporting to {ExportPath} and {ExportPathTwo}",
                    messageOutputPath, contactsOutputPath);

                JsonSerializerOptions serializerOptions = new()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };

                using FileStream messageStream = File.Create(messageOutputPath);
                await JsonSerializer.SerializeAsync(messageStream, chats, serializerOptions);
                await messageStream.DisposeAsync();

                using FileStream contactsStream = File.Create(contactsOutputPath);
                await JsonSerializer.SerializeAsync(contactsStream, contacts, serializerOptions);
                await contactsStream.DisposeAsync();
            }
            else
            {
                _logger.LogDebug("Json export is not enabled");
            }
        }
    }
}
