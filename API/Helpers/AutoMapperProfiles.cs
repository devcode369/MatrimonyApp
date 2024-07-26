namespace API.Helpers
{
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
    using API.DTOs;
    using API.Entities;
    using API.Extensions;
    using AutoMapper;
    using Microsoft.AspNetCore.Mvc.TagHelpers;

    public class AutoMapperProfiles:Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AppUser,MemberDTO>()
            .ForMember(c=>c.PhotoUrl,opt=>opt.MapFrom(src=>src.Photos.FirstOrDefault(x=>x.IsMain).Url))
            .ForMember(d=>d.Age,opt=>opt.MapFrom(src=>src.DateOfBirth.CalculateAge()));
            CreateMap<Photo,PhotoDTO>();
            CreateMap<MemberUpdateDto,AppUser>();
            CreateMap<RegisterDTO,AppUser>();
            CreateMap<Message,MessageDto>()
                .ForMember(d=>d.SenderPhotoUrl,o=>o.MapFrom(s=>s.Sender.Photos.FirstOrDefault(x=>x.IsMain).Url))
                .ForMember(d=>d.RecipientPhotoUrl,o=>o.MapFrom(s=>s.Recipient.Photos.FirstOrDefault(x=>x.IsMain).Url));
        }
    }
}