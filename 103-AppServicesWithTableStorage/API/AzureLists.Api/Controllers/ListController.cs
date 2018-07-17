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
    public class ListController : ApiController
    {
        private readonly ListService listService;

        public ListController(ListService listService)
        {
            this.listService = listService;
        }

        [HttpGet]
        [Route("api/lists")]        
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<List>))]
        public async Task<IHttpActionResult> Get()
        {
            IEnumerable<List> lists = await this.listService.Get<Library.List>();
            return this.Ok(lists);
        }

        [HttpGet]
        [Route("api/lists/{id}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List))]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public async Task<IHttpActionResult> Get(string id)
        {
            IEnumerable<Library.List> lists = await this.listService.Get<Library.List>(id);
            Library.List list = lists?.ToList()?.FirstOrDefault();
            return list != null ? this.Ok(list) : this.NotFound() as IHttpActionResult;
        }

        [HttpPost]
        [Route("api/lists")]
        [SwaggerResponse(HttpStatusCode.Created, Type = typeof(List))]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        public async Task<IHttpActionResult> Post(List list)
        {
            List l = await this.listService.Create(list);
            return this.Created($"api/lists/{l.Id}", l);
        }

        [HttpPut]
        [Route("api/lists/{id}")]
        [SwaggerResponse(HttpStatusCode.Created, Type = typeof(Library.List))]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        public async Task<IHttpActionResult> Put(string id, Library.List list)
        {
            await this.listService.Replace<Library.List>(id, list);
            return this.Created($"api/lists/{list.Id}", list);
        }

        [HttpDelete]
        [Route("api/lists/{id}")]
        [SwaggerResponse(HttpStatusCode.OK)]
        public async Task<IHttpActionResult> Delete(string id)
        {
            await this.listService.Delete<Library.List>(id);
            return this.Ok();
        }

        [HttpGet]
        [Route("api/lists/{listId}/tasks")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<Library.Task>))]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public async Task<IHttpActionResult> GetTasks(string listId, [FromUri] bool? important = null, bool? completed = null)
        {
            IEnumerable<Library.List> lists = await this.listService.Get<Library.List>(listId, important, completed);
            Library.List list = lists?.ToList()?.FirstOrDefault();
            return list != null ? this.Ok(list.Tasks) : this.NotFound() as IHttpActionResult;
        }

        [HttpGet]
        [Route("api/lists/{listId}/tasks/{taskId}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<Library.Task>))]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public async Task<IHttpActionResult> GetTask(string listId, string taskId)
        {
            IEnumerable<Library.Task> tasks = await this.listService.Get<Library.Task>(taskId);
            Library.Task task = tasks?.FirstOrDefault();
            return task != null ? this.Ok(task) : this.NotFound() as IHttpActionResult;
        }

        [HttpPost]
        [Route("api/lists/{listId}/tasks")]
        [SwaggerResponse(HttpStatusCode.Created, Type = typeof(IEnumerable<Library.Task>))]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        public async Task<IHttpActionResult> PostTask(string listId, Library.Task task)
        {
            Library.Task t = await this.listService.Create(task, listId);
            return this.Created($"api/lists/{listId}/tasks/{t.Id}", t);
        }

        [HttpPut]
        [Route("api/lists/{listId}/tasks/{taskId}")]
        [SwaggerResponse(HttpStatusCode.Created, Type = typeof(Library.Task))]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        public async Task<IHttpActionResult> Put(string listId, string taskId, Library.Task task)
        {
            await this.listService.Replace<Library.Task>(taskId, task);
            return this.Created($"api/lists/{listId}/tasks/{taskId}", task);
        }

        [HttpDelete]
        [Route("api/lists/{listId}/tasks/{taskId}")]
        [SwaggerResponse(HttpStatusCode.OK)]
        public async Task<IHttpActionResult> DeleteTask(string listId, string taskId)
        {
            await this.listService.Delete<Library.Task>(taskId);
            return this.Ok();
        }
    }    
}