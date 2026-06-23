using AutoMapper;
using CourseService.Entities;
using Shared.Models;

namespace CourseService.Mapping
{
    public class CourseMappingProfile : Profile
    {
        public CourseMappingProfile()
        {
            CreateMap<Course, CourseResponseModel>();
            CreateMap<CourseRequestModel, Course>();
            CreateMap<Enrollment, EnrollmentResponseModel>();
            CreateMap<EnrollmentRequestModel, Enrollment>();
            CreateMap<Semester, SemesterResponseModel>();
            CreateMap<SemesterRequestModel, Semester>();
            CreateMap<Subject, SubjectBusinessModel>();
            CreateMap<SubjectRequestModel, Subject>();
        }
    }
}
