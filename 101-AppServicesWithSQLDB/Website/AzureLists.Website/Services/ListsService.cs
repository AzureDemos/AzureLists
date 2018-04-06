using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace AzureLists.Website.Services
{
    public class ListsService
    {

        public async Task<List<Models.Api.List>> GetAllLists()
        {
            List<Models.Api.List> lst = new List<Models.Api.List>();

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync("http://azurelistsapi.azurewebsites.net/api/lists");
                if (response.IsSuccessStatusCode)
                {
                    var stringContent = await response.Content.ReadAsStringAsync();
                    lst = JsonConvert.DeserializeObject<List<Models.Api.List>>(stringContent);
                }
            }
            return lst;
        }
    }
}