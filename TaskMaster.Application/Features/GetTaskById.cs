using AutoMapper;
using System;
using System.Threading.Tasks;
using TaskMaster.Application.DTOs;
using TaskMaster.Application.Interfaces;

namespace TaskMaster.Application.Features
{
    public class GetTaskByIdQuery
    {
        public Guid Id { get; set; }
    }

    public class GetTaskByIdQueryHandler
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetTaskByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<TaskItemDto?> Handle(GetTaskByIdQuery query)
        {
            var task = await _unitOfWork.TaskItems.GetByIdAsync(query.Id);
            return _mapper.Map<TaskItemDto>(task);
        }
    }
}