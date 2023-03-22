using AutoMapper;
using WhatsAppConvertor.Domain.Dto;
using WhatsAppConvertor.Models;

namespace WhatsAppConvertor.Mapping
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<ChatMessage, ChatMessageDto>()
                .ForMember(dest => dest.Thumbnail, opt => opt.MapFrom(src => Convert.ToBase64String(src.Thumbnail ?? new byte[] { })))
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => new DateTime(1970, 1, 1).AddMilliseconds(src.Timestamp)));
            CreateMap<Contact, ContactDto>();
        }
    }
}
