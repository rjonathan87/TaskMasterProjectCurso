using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskMaster.Application.DTOs;
using TaskMaster.Application.Interfaces;

namespace TaskMaster.Application.Features
{
    public class GetAllTasksQuery
    {
        // Podríamos añadir filtros aquí en el futuro
    }

    public class GetAllTasksQueryHandler
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllTasksQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TaskItemDto>> Handle(GetAllTasksQuery query)
        {
            var tasks = await _unitOfWork.TaskItems.GetAllAsync();
            return _mapper.Map<IEnumerable<TaskItemDto>>(tasks);
        }
    }
}