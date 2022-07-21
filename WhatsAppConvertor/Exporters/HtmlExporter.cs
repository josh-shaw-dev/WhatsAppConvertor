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
                string outputPath = Path.Combine(_options.Directory, "output.html");

                _logger.LogInformation("Html export enabled, exporting to {ExportPath}", outputPath);

                RazorTemplateEngine.Initialize();

                List<ChatMessageDto> mappedMessages = _mapper.Map<List<ChatMessageDto>>(chats);
                List<ChatMessageAndContactDto> mappedMessagesAndContacts = _mapper.Map<List<ChatMessageAndContactDto>>(messagesWithContacts);
                ChatMessagesModel model = new()
                {
                    ChatMessages = mappedMessages,
                    Contacts = _mapper.Map<List<ContactDto>>(contacts),
                    ChatMessagesAndContacts = mappedMessagesAndContacts
                };
                string html = await RazorTemplateEngine.RenderAsync("/Views/MessageView.cshtml", model);
                using StreamWriter htmlWriter = File.CreateText(outputPath);

                await htmlWriter.WriteAsync(html);
            }
            else
            {
                _logger.LogDebug("Html export is not enabled");
            }
        }
    }
}
