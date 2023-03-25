using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Razor.Templating.Core;
using RazorTemplates.Models;
using WhatsAppConvertor.Configuration;
using WhatsAppConvertor.Domain.Dto;
using WhatsAppConvertor.Models;

namespace WhatsAppConvertor.Exporters
{
    internal class HtmlExporter : IExporter
    {
        private readonly IMapper _mapper;
        private readonly ILogger<HtmlExporter> _logger;
        private readonly ExportOptions _options;

        public HtmlExporter(
            ILogger<HtmlExporter> logger,
            IMapper mapper,
            IOptions<ExportOptions> options)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task ExportAsync(
            IEnumerable<ChatMessage> chats,
            IEnumerable<Contact> contacts,
            IList<ChatMessageAndContact> messagesWithContacts)
        {
            if (_options.Html)
            {
                char[] unwantedFileSeperators = new char[] { ' ', '\\', '/' };
                string outputDir = Path.Combine(_options.Directory, "html");
                string outputPath = Path.Combine(outputDir, "index.html");

                _logger.LogInformation("Html export enabled, exporting to {ExportPath}", outputPath);

                Directory.CreateDirectory(outputDir);

                IDictionary<string, Contact> contactsJidDict = contacts.ToDictionary(c => c.RawStringJid ?? string.Empty);
                IList<ChatGroupDto> chatGroups = new List<ChatGroupDto>();
                var groupedChats = chats.GroupBy(c => c.ChatId);
                foreach (IGrouping<int, ChatMessage> groupChat in groupedChats)
                {
                    ChatMessage? chatMessage = groupChat.FirstOrDefault(c => !string.IsNullOrWhiteSpace(c.RawStringJid));
                    contactsJidDict.TryGetValue(chatMessage?.RawStringJid ?? string.Empty, out Contact? contact);

                    string? displayName = contact?.DisplayName ?? chatMessage?.RawStringJid;
                    ChatMessagesModel messagesModel = new()
                    {
                        DisplayName = displayName,
                        ChatMessages = _mapper.Map<List<ChatMessageDto>>(groupChat.ToList())
                    };
                    ChatGroupDto chatGroup = new()
                    {
                        ChatId = groupChat.Key,
                        DisplayName = displayName
                    };
                    chatGroups.Add(chatGroup);

                    string outputChatPath = Path.Combine(outputDir, chatGroup.FilePath);
                    Directory.CreateDirectory(outputChatPath);

                    // Write the messages to html files
                    string messageHtmlOutputPath = Path.Combine(outputChatPath, $"chat-{chatGroup.FilePath}.html");
                    string messageHtml = await RazorTemplateEngine.RenderAsync("/Views/MessagesView.cshtml", messagesModel);

                    await WriteFileAsync(messageHtmlOutputPath, messageHtml);
                }

                ChatGroupModel model = new()
                {
                    ChatGroups = chatGroups
                };
                string html = await RazorTemplateEngine.RenderAsync("/Views/ChatGroupView.cshtml", model);

                await WriteFileAsync(outputPath, html);
            }
            else
            {
                _logger.LogDebug("Html export is not enabled");
            }
        }

        private static async Task WriteFileAsync(string filePath, string fileContent)
        {
            using StreamWriter htmlWriter = File.CreateText(filePath);

            await htmlWriter.WriteAsync(fileContent);
        }
    }
}
