using System.IO.Abstractions;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WhatsAppConvertor.Configuration;
using WhatsAppConvertor.Models;

namespace WhatsAppConvertor.Exporters
{
    internal class TextExporter : IExporter
    {
        private readonly ILogger<TextExporter> _logger;
        private readonly ExportOptions _options;
        private readonly IFileSystem _fileSystem;

        public TextExporter(
            ILogger<TextExporter> logger,
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
            if (_options.Text.Enabled)
            {
                string outputPath = Path.Combine(_options.Directory, "output.txt");

                _logger.LogInformation("Text export enabled, exporting to {ExportPath}", outputPath);

                _fileSystem.Directory.CreateDirectory(_options.Directory);
                using FileSystemStream? fileStream = _fileSystem.FileStream.New(outputPath, FileMode.OpenOrCreate);
                using StreamWriter streamWriter = new(fileStream, Encoding.UTF8);

                foreach (ChatMessageAndContact chatMessage in messagesWithContacts)
                {
                    ChatMessage? message = chatMessage.ChatMessage;
                    Contact? contact = chatMessage.Contact;

                    if (message != null)
                    {
                        DateTimeOffset messageRecievedTime = DateTimeOffset.FromUnixTimeMilliseconds(message.Timestamp);
                        string? from = message.MessageFromMe ? "Me" : contact?.DisplayName ?? contact?.RawStringJid;
                        string? messageText = message.FilePath ?? message.MessageText;

                        string messageCat = $"{messageRecievedTime:s} - {from} - {messageText}";
                        await streamWriter.WriteLineAsync(messageCat);
                    }
                }
            }
            else
            {
                _logger.LogDebug("Html export is not enabled");
            }
        }
    }
}
