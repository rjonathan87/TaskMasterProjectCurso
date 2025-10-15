using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskMaster.Application.DTOs;
using TaskMaster.Application.Interfaces;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

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

        private readonly IMemoryCache _cache;
        private readonly ILogger<GetAllTasksQueryHandler> _logger;

        public GetAllTasksQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IMemoryCache cache, ILogger<GetAllTasksQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;

            _cache = cache;
            _logger = logger;
        }

        public async Task<IEnumerable<TaskItemDto>> Handle(GetAllTasksQuery query)
        {
            //var tasks = await _unitOfWork.TaskItems.GetAllAsync();
            //return _mapper.Map<IEnumerable<TaskItemDto>>(tasks);

            const string cacheKey = "AllTasksCacheKey";

            // Intentamos obtener los datos de la caché
            if (_cache.TryGetValue(cacheKey, out IEnumerable<TaskItemDto> cachedTasks))
            {
                _logger.LogInformation("Datos de tareas obtenidos de la caché.");
                return cachedTasks!;
            }

            // Si no están en caché, los obtenemos de la base de datos
            _logger.LogInformation("Datos de tareas no encontrados en caché. Obteniendo de la base de datos.");
            var tasks = await _unitOfWork.TaskItems.GetAllAsync();
            var tasksDto = _mapper.Map<IEnumerable<TaskItemDto>>(tasks);

            // Configuramos las opciones de la caché
            var cacheOptions = new MemoryCacheEntryOptions()
                // SlidingExpiration: El item expira si no se accede a él en un tiempo determinado (ej. 2 minutos).
                // Cada acceso "resetea" el contador. Ideal para datos que se usan con frecuencia.
                // Alternativa: SetAbsoluteExpiration, que expira después de un tiempo fijo, sin importar los accesos.
                .SetSlidingExpiration(TimeSpan.FromMinutes(2));

            // Guardamos los datos en la caché
            _cache.Set(cacheKey, tasksDto, cacheOptions);

            return tasksDto;
        }
    }
}