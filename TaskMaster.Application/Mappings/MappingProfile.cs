using AutoMapper;
using TaskMaster.Application.DTOs;
using TaskMaster.Domain.Entities;

namespace TaskMaster.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TaskItem, TaskItemDto>().ReverseMap();
            CreateMap<CreateTaskRequest, TaskItem>();
            CreateMap<UpdateTaskRequest, TaskItem>();
        }
    }
}