using AzureLists.Library;
using AzureLists.TableStorage;
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
    public class DummyDataController : ApiController
    {
        private readonly IListService listService;

        public DummyDataController(IListService listService)
        {
            this.listService = listService;
        }

        [HttpGet]
        [Route("api/dummydata")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(string))]
        public async Task<IHttpActionResult> Get()
        {
            IEnumerable<List> lists = await this.listService.GetAllLists();
            foreach (var l in lists)
                await this.listService.DeleteList(l.Id);

            var dummyDataGenerator = new DummyDataGenerator(); //ideally this should really be injected via the constructor
            var inbox = dummyDataGenerator.CreateInboxList();
            var groceries = dummyDataGenerator.CreateGroceriesList();
            var holiday = dummyDataGenerator.CreateHolidayList();

            await this.listService.CreateOrUpdateList(inbox);
            await this.listService.CreateOrUpdateList(groceries);
            await this.listService.CreateOrUpdateList(holiday);

            return Ok("All existsing lists deleted, and three new lists created");
        }


    }
}
