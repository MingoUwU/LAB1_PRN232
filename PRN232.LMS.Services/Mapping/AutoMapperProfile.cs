using AutoMapper;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Services.Models;

namespace PRN232.LMS.Services.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Student, StudentBusinessModel>().ReverseMap();
            CreateMap<Student, StudentResponseModel>();
            CreateMap<StudentRequestModel, Student>();

            CreateMap<Course, CourseBusinessModel>().ReverseMap();
            CreateMap<Course, CourseResponseModel>();
            CreateMap<CourseRequestModel, Course>();

            CreateMap<Semester, SemesterBusinessModel>().ReverseMap();
            CreateMap<Semester, SemesterResponseModel>();
            CreateMap<SemesterRequestModel, Semester>();

            CreateMap<Subject, SubjectBusinessModel>().ReverseMap();
            CreateMap<Subject, SubjectResponseModel>();
            CreateMap<SubjectRequestModel, Subject>();

            CreateMap<Enrollment, EnrollmentBusinessModel>();

            CreateMap<Enrollment, EnrollmentResponseModel>()
                .IncludeBase<Enrollment, EnrollmentBusinessModel>();
            
            CreateMap<EnrollmentRequestModel, Enrollment>();
        }
    }
}
