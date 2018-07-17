using AzureLists.Library;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace AzureLists.Api.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class TaskController : ApiController
    {
        private readonly ListService listService;

        public TaskController(ListService listService)
        {
            this.listService = listService;
        }

        [HttpGet]
        [Route("api/tasks/{id}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Library.Task))]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public async Task<IHttpActionResult> Get(string id)
        {
            IEnumerable<Library.Task> tasks = await this.listService.Get<Library.Task>(id);
            Library.Task task = tasks?.FirstOrDefault();
            return task != null ? this.Ok(task) : this.NotFound() as IHttpActionResult;
        }

        [HttpGet]
        [Route("api/tasks")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<Library.Task>))]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public async Task<IHttpActionResult> Get([FromUri] bool? important = null, bool? completed = null)
        {
            IEnumerable<Library.Task> tasks = await this.listService.Get<Library.Task>(important, completed);
            return tasks != null ? this.Ok(tasks) : this.NotFound() as IHttpActionResult;
        }

        [HttpPut]
        [Route("api/tasks/{id}")]
        [SwaggerResponse(HttpStatusCode.Created, Type = typeof(Library.Task))]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        public async Task<IHttpActionResult> Put(string id, Library.Task task)
        {
            await this.listService.Replace<Library.Task>(id, task);
            return this.Created($"api/tasks/{id}", task);
        }

        [HttpDelete]
        [Route("api/tasks/{id}")]
        [SwaggerResponse(HttpStatusCode.OK)]
        public async Task<IHttpActionResult> DeleteTask(string id)
        {
            await this.listService.Delete<Library.Task>(id);
            return this.Ok();
        }
    }
}
