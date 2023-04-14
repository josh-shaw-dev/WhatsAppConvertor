using AutoFixture.Xunit2;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.IO.Abstractions;
using WhatsAppConvertor.Configuration;
using WhatsAppConvertor.Exporters;
using WhatsAppConvertor.Models;
using WhatsAppConvertorTests.Common;

namespace WhatsAppConvertorTests.Exporters
{
    public class HtmlExporterTests
    {
        [Theory, AutoMoqData]
        public async Task ExportAsync_ShouldNotExport_WhenHtmlIsDisabled(
            [Frozen] ILogger<HtmlExporter> logger,
            [Frozen] IOptions<ExportOptions> iOptions,
            List<ChatMessage> chats,
            List<Contact> contacts,
            List<ChatMessageAndContact> messagesWithContacts,
            HtmlExporter sut)
        {
            iOptions.Value.Html.Enabled = false;

            await sut.ExportAsync(chats, contacts, messagesWithContacts);

            logger.AsMock()
                .VerifyLogWithLogLevel(LogLevel.Debug, Times.Once);
        }

        [Theory, AutoMoqData]
        public async Task ExportAsync_ShouldExport_WhenHtmlIsEnabled(
            [Frozen] IOptions<ExportOptions> iOptions,
            [Frozen] IFileSystem fileSystem,
            List<ChatMessage> chats,
            List<Contact> contacts,
            List<ChatMessageAndContact> messagesWithContacts,
            HtmlExporter sut)
        {
            iOptions.Value.Html.Enabled = true;

            await sut.ExportAsync(chats, contacts, messagesWithContacts);

            fileSystem.AsMock().Verify(d => d.Directory.CreateDirectory(It.IsAny<string>()), Times.Once);
        }
    }
}