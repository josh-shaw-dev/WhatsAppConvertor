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
                .ForMember(dest => dest.MessageFrom, opt => opt.MapFrom(src => src.MessageFromMe ? "Me" : "Them"))
                .ForMember(dest => dest.MessageText, opt => opt.MapFrom(src => src.MessageText ?? src.FilePath));
            CreateMap<Contact, ContactDto>();
            CreateMap<ChatMessageAndContact, ChatMessageAndContactDto>();
        }
    }
}
