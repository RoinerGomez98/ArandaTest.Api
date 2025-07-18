using ArandaTest.Application.DTOs;
using ArandaTest.Domain.Entities;
using AutoMapper;

namespace ArandaTest.Application.Mappers
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<Products, ProductDto>().ReverseMap();
            CreateMap<Products, ProductCreateDto>().ReverseMap();
            CreateMap<Products, ProducUpdateDto>().ReverseMap();
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<Users, UsersDto>().ReverseMap();
        }
    }
}
