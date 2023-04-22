using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using AutoFixture;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WhatsAppConvertor.Configuration;
using WhatsAppConvertor.Exporters;

namespace WhatsAppConvertorTests.Exporters.Customizations
{
    public class MockedSetupHtmlExporter : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Register<MockFileSystem>(() => new MockFileSystem());

            fixture.Register<HtmlExporter>(() => {
                ILogger<HtmlExporter> logger = fixture.Freeze<ILogger<HtmlExporter>>();
                IMapper mapper = fixture.Freeze<IMapper>();
                IOptions<ExportOptions> iOptions = fixture.Freeze<IOptions<ExportOptions>>();
                IFileSystem fileSystem = fixture.Freeze<MockFileSystem>();

                return new HtmlExporter(logger, mapper, iOptions, fileSystem);
            });
        }   
    }
}