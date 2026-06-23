using AutoMapper;
using StudentService.Entities;
using Shared.Models;

namespace StudentService.Mapping
{
    public class StudentMappingProfile : Profile
    {
        public StudentMappingProfile()
        {
            CreateMap<Student, StudentResponseModel>();
            CreateMap<StudentRequestModel, Student>();
        }
    }
}
