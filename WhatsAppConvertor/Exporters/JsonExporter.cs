using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.IO.Abstractions;
using System.Text;
using System.Text.Json;
using WhatsAppConvertor.Configuration;
using WhatsAppConvertor.Models;

namespace WhatsAppConvertor.Exporters
{
    internal class JsonExporter : IExporter
    {
        private readonly ILogger<JsonExporter> _logger;
        private readonly ExportOptions _options;
        private readonly IFileSystem _fileSystem;

        public JsonExporter(
            ILogger<JsonExporter> logger,
            IOptions<ExportOptions> options,
            IFileSystem fileSystem)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }

        public async Task ExportAsync(
            IEnumerable<ChatMessage> chats,
            IEnumerable<Contact> contacts,
            IList<ChatMessageAndContact> messagesWithContacts)
        {
            if (_options.Json.Enabled)
            {
                string messageOutputPath = Path.Combine(_options.Directory, "messages.json");
                string contactsOutputPath = Path.Combine(_options.Directory, "contacts.json");

                _logger.LogInformation("Text export enabled, exporting to {ExportPath} and {ExportPathTwo}",
                    messageOutputPath, contactsOutputPath);

                _fileSystem.Directory.CreateDirectory(_options.Directory);
                JsonSerializerOptions serializerOptions = new()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };

                using FileSystemStream? messageStream = _fileSystem.FileStream.New(messageOutputPath, FileMode.OpenOrCreate);
                await JsonSerializer.SerializeAsync(messageStream, chats, serializerOptions);

                using FileSystemStream? contactsStream = _fileSystem.FileStream.New(contactsOutputPath, FileMode.OpenOrCreate);
                await JsonSerializer.SerializeAsync(contactsStream, chats, serializerOptions);
            }
            else
            {
                _logger.LogDebug("Json export is not enabled");
            }
        }
    }
}
