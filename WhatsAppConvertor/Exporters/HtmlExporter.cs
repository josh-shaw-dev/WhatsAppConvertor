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
            if (_options.Html.Enabled)
            {
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

                    if (_options.Html.CopyMedia)
                    {
                        await CopyMedia(groupChat, outputChatPath);
                    }

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

        private async Task CopyMedia(IGrouping<int, ChatMessage> groupChat, string outputChatPath)
        {
            string mediaBasePath = _options.Html.MediaPath ?? string.Empty;

            if (string.IsNullOrWhiteSpace(mediaBasePath))
            {
                throw new ArgumentException(
                    $"Media path cannot be empty or null when {nameof(HtmlOptions.MediaPath)} is enabled",
                    _options.Html.MediaPath);
            }

            if (!Directory.Exists(mediaBasePath))
            {
                throw new DirectoryNotFoundException("Media directory does not exist");
            }

            foreach (ChatMessage message in groupChat)
            {
                if (!string.IsNullOrWhiteSpace(message.FilePath))
                {
                    string fullSrcFilePath = Path.Combine(mediaBasePath, message.FilePath);
                    string fullDestFilePath = Path.Combine(outputChatPath, message.FilePath);

                    string? destDirectory = Path.GetDirectoryName(fullDestFilePath);
                    if (!string.IsNullOrWhiteSpace(destDirectory))
                    {
                        Directory.CreateDirectory(destDirectory);
                    }

                    if (File.Exists(fullSrcFilePath))
                    {
                        using FileStream sourceStream = File.Open(fullSrcFilePath, FileMode.Open);
                        using FileStream destinationStream = File.Create(fullDestFilePath);
                        await sourceStream.CopyToAsync(destinationStream);
                    }
                    else
                    {
                        _logger.LogDebug("Source file does not exist: {SrcFilePath}", fullSrcFilePath);
                    }
                }
            }
        }

        private static async Task WriteFileAsync(string filePath, string fileContent)
        {
            using StreamWriter htmlWriter = File.CreateText(filePath);

            await htmlWriter.WriteAsync(fileContent);
        }
    }
}
