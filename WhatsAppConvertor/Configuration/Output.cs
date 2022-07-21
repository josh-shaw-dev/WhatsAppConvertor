namespace WhatsAppConvertor.Configuration
{
    public class ExportOptions
    {
        public const string Position = "Export";

        public string Directory { get; set; } = string.Empty;
        
        public bool Json { get; set; }
        
        public bool Text { get; set; }
        
        public bool Html { get; set; }
    }
}


