using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Razor.Templating.Core;
using RazorTemplates.Models;
using System.Text.Json;
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
                /*
                 * TODO I think for large amount of messages I should serialize them to disk
                 * then have the browser load in each chat when its needed
                 * It won't be a easy to share single file though will be much nicer on the browser to load
                 */

                string outputDir = Path.Combine(_options.Directory, "html");
                string outputPath = Path.Combine(outputDir, "output.html");
                _logger.LogInformation("Html export enabled, exporting to {ExportPath}", outputPath);

                Directory.CreateDirectory(outputDir);
                RazorTemplateEngine.Initialize();

                IDictionary<string?, Contact> contactsJidDict = contacts.ToDictionary(c => c.RawStringJid);
                IList<ChatGroupDto> chatGroups = new List<ChatGroupDto>();
                var groupedChats = chats.GroupBy(c => c.ChatId);
                foreach (IGrouping<int, ChatMessage> groupChat in groupedChats)
                {
                    ChatMessage? chatMessage = groupChat.FirstOrDefault(c => !string.IsNullOrWhiteSpace(c.RawStringJid));
                    contactsJidDict.TryGetValue(chatMessage?.RawStringJid ?? string.Empty, out Contact? contact);

                    ChatGroupDto chatGroup = new()
                    {
                        ChatId = groupChat.Key,
                        DisplayName = contact?.DisplayName ?? chatMessage?.RawStringJid
                    };
                    chatGroups.Add(chatGroup);
                }

                ChatMessagesModel model = new()
                {
                    ChatGroups = chatGroups
                };
                string html = await RazorTemplateEngine.RenderAsync("/Views/MessageView.cshtml", model);
                using StreamWriter htmlWriter = File.CreateText(outputPath);

                await htmlWriter.WriteAsync(html);

                await SerializeData(outputDir, chats);
            }
            else
            {
                _logger.LogDebug("Html export is not enabled");
            }
        }

        private static async Task SerializeData(string outputBaseDirectory, IEnumerable<ChatMessage> chats)
        {
            JsonSerializerOptions serializerOptions = new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            IEnumerable<IGrouping<int, ChatMessage>> groupedChats = chats.GroupBy(c => c.ChatId);
            string outputChatDirectory = Path.Combine(outputBaseDirectory, "chats");
            Directory.CreateDirectory(outputChatDirectory);

            foreach (IGrouping<int, ChatMessage> groupChat in groupedChats)
            {
                int chatId = groupChat.Key;
                string chatOutputPath = Path.Combine(outputChatDirectory, $"chat-{chatId}.json");

                using FileStream messageStream = File.Create(chatOutputPath);
                await JsonSerializer.SerializeAsync(messageStream, groupChat, serializerOptions);
                await messageStream.DisposeAsync();
            }
        }
    }
}
