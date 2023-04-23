using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Razor.Templating.Core;
using RazorTemplates.Models;
using System.IO.Abstractions;
using System.Text;
using WhatsAppConvertor.Configuration;
using WhatsAppConvertor.Domain.Dto;
using WhatsAppConvertor.Models;

namespace WhatsAppConvertor.Exporters
{
    public class HtmlExporter : IExporter
    {
        private readonly ILogger<HtmlExporter> _logger;
        private readonly IMapper _mapper;
        private readonly ExportOptions _options;
        private readonly IFileSystem _fileSystem;

        public HtmlExporter(
            ILogger<HtmlExporter> logger,
            IMapper mapper,
            IOptions<ExportOptions> options,
            IFileSystem fileSystem)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
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

                _fileSystem.Directory.CreateDirectory(outputDir);

                IDictionary<string, Contact> contactsJidDict = contacts.ToDictionary(c => c.RawStringJid ?? string.Empty);
                IList<ChatGroupDto> chatGroups = new List<ChatGroupDto>();
                IEnumerable<IGrouping<int, ChatMessage>> groupedChats = chats.GroupBy(c => c.ChatId);
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
                    _fileSystem.Directory.CreateDirectory(outputChatPath);

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
                        _fileSystem.Directory.CreateDirectory(destDirectory);
                    }

                    if (File.Exists(fullSrcFilePath))
                    {
                        using FileSystemStream sourceStream = _fileSystem.File.Open(fullSrcFilePath, FileMode.Open);
                        using FileSystemStream destinationStream = _fileSystem.File.Create(fullDestFilePath);
                        await sourceStream.CopyToAsync(destinationStream);
                    }
                    else
                    {
                        _logger.LogDebug("Source file does not exist: {SrcFilePath}", fullSrcFilePath);
                    }
                }
            }
        }

        private async Task WriteFileAsync(string filePath, string fileContent)
        {
            using FileSystemStream? fileStream = _fileSystem.FileStream.New(filePath, FileMode.OpenOrCreate);
            using StreamWriter streamWriter = new(fileStream, Encoding.UTF8);
            await streamWriter.WriteAsync(fileContent);
        }
    }
}
