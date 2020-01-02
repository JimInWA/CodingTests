using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASPNetCoreWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace ASPNetCoreWebApi.Controllers
{
    [Authorize]
    [AllowAnonymous]
    [Route("[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        // ToDo: Look into the ITask issue, see comments above CreateTask, UpdateTask, and DeleteTask methods
        // ToDo: Introduce a Processor class (based on an Interface) and move all of the EntityFramework-related logic to that class; will allow us to write unit tests for these API methods
        // ToDo: Write custom exception type to override to StackTrace.ToString() to prevent stack traces from leaking in non-Development scenarios
        // ToDo: Investigate the comments about the overposting attacks and determine what would need to be done in a non-Development scenario
        // ToDo: Introduce logging

        private readonly SimpleTasksContext _context;

        public TasksController(SimpleTasksContext context)
        {
            _context = context;
        }

        // GET: Tasks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Models.ITask>>> GetSimpleTasks()
        {
            try
            {
                return await _context.SimpleTasks.OrderBy(x => x.Id).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(GetSimpleTasks)} threw an exception: {ex}");
            }
        }

        // GET: Tasks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Models.ITask>> GetTask(int id)
        {
            try
            {
                var task = await _context.SimpleTasks.FindAsync(id);

                if (task == null)
                {
                    return NotFound();
                }

                return task;
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(GetTask)} threw an exception: {ex}");
            }
        }

        // POST: Tasks
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        // When trying to using Models.ITask via swagger - System.NotSupportedException: Deserialization of interface types is not supported. Type 'ASPNetCoreWebApi.Models.ITask'
        // https://github.com/dotnet/corefx/issues/41325
        // https://github.com/dotnet/corefx/issues/39031
        [HttpPost]
        public async Task<ActionResult<int>> CreateTask(Models.Task task)
        {
            try
            {
                // ToDo: If we are able to get the SimpleTasks to use an interface, update the signature of this method to use ITask
                _context.SimpleTasks.Add(task);
                await _context.SaveChangesAsync();

                int newId = task.Id;
                return newId;
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(CreateTask)} threw an exception: {ex}");
            }
        }

        // PUT: Tasks/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        // When trying to using Models.ITask via swagger worked but then when the call came from the client got an error - System.NotSupportedException: Deserialization of interface types is not supported. Type 'ASPNetCoreWebApi.Models.ITask'
        // https://github.com/dotnet/corefx/issues/41325
        // https://github.com/dotnet/corefx/issues/39031
        [HttpPut("{id}")]
        public async Task<ActionResult<bool>> UpdateTask(int id, Models.Task task)
        {
            if (id != task.Id)
            {
                return BadRequest();
            }

            try
            {
                _context.Entry(task).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!TaskExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw new Exception($"{nameof(UpdateTask)} threw an exception: {ex}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(UpdateTask)} threw an exception: {ex}");
            }

            return true;
        }

        // DELETE: Tasks/5
        // When trying to using Models.ITask via swagger - System.NotSupportedException: Deserialization of interface types is not supported. Type 'ASPNetCoreWebApi.Models.ITask'
        // https://github.com/dotnet/corefx/issues/41325
        // https://github.com/dotnet/corefx/issues/39031
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteTask(int id, Models.Task task)
        {
            if (id != task.Id)
            {
                return BadRequest();
            }

            try
            {
                _context.SimpleTasks.Remove(task);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!TaskExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw new Exception($"{nameof(DeleteTask)} threw an exception: {ex}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(DeleteTask)} threw an exception: {ex}");
            }

            return true;
        }

        private bool TaskExists(int id)
        {
            return _context.SimpleTasks.Any(e => e.Id == id);
        }
    }
}
