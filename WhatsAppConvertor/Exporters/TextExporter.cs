using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WhatsAppConvertor.Configuration;
using WhatsAppConvertor.Models;

namespace WhatsAppConvertor.Exporters
{
    internal class TextExporter : IExporter
    {
        private readonly ILogger<HtmlExporter> _logger;
        private readonly ExportOptions _options;

        public TextExporter(
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
            if (_options.Text)
            {
                string outputPath = Path.Combine(_options.Directory, "output.txt");

                _logger.LogInformation("Text export enabled, exporting to {ExportPath}", outputPath);

                using StreamWriter writer = File.CreateText(outputPath);

                foreach (ChatMessageAndContact chatMessage in messagesWithContacts)
                {
                    ChatMessage? message = chatMessage.ChatMessage;
                    Contact? contact = chatMessage.Contact;

                    if (message != null)
                    {
                        DateTimeOffset messageRecievedTime = DateTimeOffset.FromUnixTimeMilliseconds(message.MessageRecievedTime);
                        string? from = message.MessageFromMe ? "Me" : contact?.DisplayName ?? contact?.RawStringJid;
                        string? messageText = message.MediaFilePath ?? message.MessageText;

                        string messageCat = $"{messageRecievedTime:s} - {from} - {messageText}";
                        await writer.WriteLineAsync(messageCat);
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
