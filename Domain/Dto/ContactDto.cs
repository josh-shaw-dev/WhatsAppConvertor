namespace WhatsAppConvertor.Domain.Dto
{
    public class ContactDto
    {
        public bool IsWhatsAppUser { get; set; }
        
        public string? Status { get; set; }

        public string? DisplayName { get; set; }

        public string? GivenName { get; set; }

        public string? FamilyName { get; set; }

        public string? SortName { get; set; }
    }
}
