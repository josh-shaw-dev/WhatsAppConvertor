namespace WhatsAppConvertor.Configuration
{
    public class ExportOptions
    {
        public const string Position = "Export";

        public string Directory { get; set; } = string.Empty;

        public BasicExportOptions Json { get; set; } = new();
        
        public BasicExportOptions Text { get; set; } = new();

        public HtmlOptions Html { get; set; } = new();
    }

    public class BasicExportOptions
    {
        public bool Enabled { get; set; }
    }

    public class HtmlOptions : BasicExportOptions
    {
        public bool CopyMedia { get; set; }

        public string? MediaPath { get; set; }
    }
}
