using AutoMapper;
using Friends_Date_API.DTO;
using Friends_Date_API.Entities;
using Friends_Date_API.Extension;
using System;
using System.Linq;

namespace Friends_Date_API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            // if we want to map individual property 
            CreateMap<User, MemberDto>()
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src =>
                     src.Photos.FirstOrDefault(x => x.IsMain).Url))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src =>
                     src.DateOfBirth.CalculateAge()));

            CreateMap<Photo, PhotoDto>();
            CreateMap<MemberUpdateDto, User>();
            CreateMap<RegisterDto, User>();
            CreateMap<Message, MessageDto>()
                  .ForMember(dest => dest.SenderPhtoUrl, opt => opt.MapFrom(src =>
                         src.Sender.Photos.FirstOrDefault(x => x.IsMain).Url))
                  .ForMember(dest => dest.RecipientPhotoUrl, opt => opt.MapFrom(src =>
                         src.Recipient.Photos.FirstOrDefault(x => x.IsMain).Url));

            // this is basically means when we return date to our client we will have the z in the end of datetime
            // which will provide our local time according to the time zone
            // we convert our date time in datacontext we dont' need this
            //CreateMap<DateTime, DateTime>().ConvertUsing(d => DateTime.SpecifyKind(d, DateTimeKind.Utc));
        }
    }
}
