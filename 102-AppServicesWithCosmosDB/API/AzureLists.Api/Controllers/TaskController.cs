using AzureLists.Library;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace AzureLists.Api.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class TaskController : ApiController
    {
        private readonly IListService listService;

        public TaskController(IListService listService)
        {
            this.listService = listService;
        }

        [HttpGet]
        [Route("api/tasks/{id}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Library.Task))]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public async Task<IHttpActionResult> Get(string id)
        {
            var task = await this.listService.GetTaskById(id);
            return task != null ? this.Ok(task) : this.NotFound() as IHttpActionResult;
        }

        [HttpGet]
        [Route("api/tasks")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<Library.Task>))]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public async Task<IHttpActionResult> Get([FromUri] bool? important = null, bool? completed = null)
        {
            var tasks = await this.listService.SearchTasks(important: important, completed: completed);
            return tasks != null ? this.Ok(tasks) : this.NotFound() as IHttpActionResult;
        }

        [HttpPut]
        [Route("api/tasks/{id}")]
        [SwaggerResponse(HttpStatusCode.Created, Type = typeof(Library.Task))]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        public async Task<IHttpActionResult> Put(string id, Library.Task task)
        {
            await this.listService.ReplaceTask(task);
            return this.Created($"api/tasks/{id}", task);
        }

        [HttpDelete]
        [Route("api/tasks/{id}")]
        [SwaggerResponse(HttpStatusCode.OK)]
        public async Task<IHttpActionResult> DeleteTask(string id)
        {
            await this.listService.DeleteTask(id);
            return this.Ok();
        }
    }
}
