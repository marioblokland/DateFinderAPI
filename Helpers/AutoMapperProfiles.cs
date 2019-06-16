using System.Linq;
using AutoMapper;
using DatingApp.API.Dto;
using DatingApp.API.Models;

namespace DatingApp.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForDetailedDto>().ForMember(destination => destination.PhotoUrl,
                    options => { options.MapFrom(source => source.Photos.FirstOrDefault(photo => photo.IsMain).Url); })
                .ForMember(destination => destination.Age,
                    options => { options.MapFrom(person => person.DateOfBirth.CalculateAgeFromDate()); });

            CreateMap<User, UserForListDto>().ForMember(destination => destination.PhotoUrl,
                    options => { options.MapFrom(source => source.Photos.FirstOrDefault(photo => photo.IsMain).Url); })
                .ForMember(destination => destination.Age,
                    options => { options.MapFrom(person => person.DateOfBirth.CalculateAgeFromDate()); });

            CreateMap<Photo, PhotosForDetailedDto>();
            CreateMap<UserForUpdateDto, User>();
        }
    }
}
