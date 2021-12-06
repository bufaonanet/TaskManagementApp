using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagementApp.Application.Interfaces;

namespace TaskManagementApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public TasksController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("{id:int}",Name ="GetTaskById")]
        public async Task<ActionResult<IEnumerable<Core.Entities.Task>>> Get(int id)
        {
            var task = await _unitOfWork.Tasks.Get(id);
            if (task is null)
            {
                return NotFound();
            }

            return Ok(task);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Core.Entities.Task>>> GetAll()
        {
            var tasks = await _unitOfWork.Tasks.GetAll();

            return Ok(tasks);
        }

        [HttpPost()]
        public async Task<ActionResult<Core.Entities.Task>> Post([FromBody] Core.Entities.Task task)
        {
            try
            {
                var result = await _unitOfWork.Tasks.Add(task);

                if (result > 0)
                {
                    return new CreatedAtRouteResult("GetTaskById", new { id = task.Id }, task);
                }

                return BadRequest();

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                   "Erro ao criar uma Task");
            }
        }

        [HttpPut("{id:int}")]       
        public async Task<ActionResult> Put(int id, [FromBody] Core.Entities.Task task)
        {
            try
            {
                if (id != task.Id)
                {
                    return BadRequest($"Não foi possível atualizar Task com id={id}");
                }


                var result = await _unitOfWork.Tasks.Update(task);

                if (result > 0)
                {
                    return Ok($"Task com id={id} atualizada com sucesso!");
                }

                return BadRequest();
               
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Erro tentar atualizar Task com id={id}");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var task = await _unitOfWork.Tasks.Get(id);              

                if (task == null)
                {
                    return NotFound($"Nenhuma Task para o Id={id}");
                }

                var result = await _unitOfWork.Tasks.Delete(id);

                if (result > 0)
                {
                    return Ok($"Task com id={id} deletada com sucesso!");
                }

                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Erro tentar excluir Categoria com id={id}");
            }
        }
    }
}