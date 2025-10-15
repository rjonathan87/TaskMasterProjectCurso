using FluentAssertions;
using Moq;
using System;
using System.Threading.Tasks;
using TaskMaster.Application.DTOs;
using TaskMaster.Application.Features;
using TaskMaster.Application.Interfaces;
using TaskMaster.Domain.Entities;
using Xunit;

namespace TaskMaster.Tests.Application.Features
{
    public class CreateTaskTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly CreateTaskCommandHandler _handler;

        public CreateTaskTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            // El handler real no usa AutoMapper, así que no necesitamos mockearlo aquí.
            _handler = new CreateTaskCommandHandler(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_Should_CreateAndReturnTask_WhenRequestIsValid()
        {
            // 1. ARRANGE (Preparar)

            // El DTO de solicitud que simula la entrada de la API.
            var request = new CreateTaskRequest
            {
                Title = "Test Title",
                Description = "Test Description",
                ProjectId = Guid.NewGuid(),
                AssignedToId = null, // O un Guid si es necesario
                DueDate = DateTime.UtcNow.AddDays(5)
            };

            // Preparamos el mock del repositorio genérico para TaskItem.
            var mockTaskRepository = new Mock<IGenericRepository<TaskItem>>();
            
            // Configuramos el mock de Unit of Work para que devuelva nuestro mock de repositorio.
            _mockUnitOfWork.Setup(uow => uow.TaskItems).Returns(mockTaskRepository.Object);

            // Configuramos el mock para que el guardado devuelva 1 (simulando una fila afectada).
            _mockUnitOfWork.Setup(uow => uow.CompleteAsync()).ReturnsAsync(1);

            // 2. ACT (Actuar)
            var result = await _handler.Handle(request);

            // 3. ASSERT (Verificar)

            // El resultado no debe ser nulo.
            result.Should().NotBeNull();
            
            // El título y la descripción en el resultado deben coincidir con la solicitud.
            result.Title.Should().Be(request.Title);
            result.Description.Should().Be(request.Description);
            
            // El estado inicial debe ser 'ToDo'.
            result.Status.Should().Be(TaskMaster.Domain.Entities.TaskStatus.ToDo);

            // Verificar que el método AddAsync fue llamado en el repositorio de tareas exactamente una vez.
            mockTaskRepository.Verify(repo => repo.AddAsync(It.Is<TaskItem>(task => 
                task.Title == request.Title && task.Description == request.Description
            )), Times.Once);

            // Verificar que el método CompleteAsync (guardado) fue llamado en la unidad de trabajo exactamente una vez.
            _mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Once);
        }
    }
}