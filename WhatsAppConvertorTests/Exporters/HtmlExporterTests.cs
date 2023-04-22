using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.IO.Abstractions.TestingHelpers;
using WhatsAppConvertor.Configuration;
using WhatsAppConvertor.Domain.Dto;
using WhatsAppConvertor.Exporters;
using WhatsAppConvertor.Models;
using WhatsAppConvertorTests.Common;
using WhatsAppConvertorTests.Exporters.Customizations;

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
                .VerifyLogWithLogLevelAndContainsMessage(LogLevel.Debug, Times.Once, "export is not enabled");
        }

        [Theory, AutoMoqData(typeof(MockedSetupHtmlExporter))]
        public async Task ExportAsync_HtmlExportTrue_CreatesTheConfiguredBaseDirectory(
            [Frozen] IOptions<ExportOptions> iOptions,
            [Frozen] MockFileSystem fileSystem,
            List<ChatMessage> chats,
            List<Contact> contacts,
            List<ChatMessageAndContact> messagesWithContacts,
            string baseDirectory,
            HtmlExporter sut)
        {
            baseDirectory = GetBaseDirectory(baseDirectory);
            iOptions.Value.Html.Enabled = true;
            iOptions.Value.Directory = baseDirectory;

            await sut.ExportAsync(chats, contacts, messagesWithContacts);

            fileSystem.AllDirectories.Should()
                .Contain(dir => dir.StartsWith(baseDirectory), "because it should have created a base directory");
        }

        [Theory, AutoMoqData(typeof(MockedSetupHtmlExporter))]
        public async Task ExportAsync_HtmlExportTrue_CreatesExpectedFiles(
            [Frozen] IOptions<ExportOptions> iOptions,
            [Frozen] MockFileSystem fileSystem,
            List<ChatMessage> chats,
            List<Contact> contacts,
            List<ChatMessageAndContact> messagesWithContacts,
            HtmlExporter sut)
        {
            iOptions.Value.Html.Enabled = true;

            await sut.ExportAsync(chats, contacts, messagesWithContacts);

            fileSystem.AllFiles.Should()
                .Contain(file => chats.Any(c => file.Contains(chats.First().ChatId.ToString())));
        }

        private static string GetBaseDirectory(string path)
        {
            if (path.StartsWith(Path.DirectorySeparatorChar)) {
                return path;
            }

            return $"{Path.DirectorySeparatorChar}{path}";
        }
    }
}