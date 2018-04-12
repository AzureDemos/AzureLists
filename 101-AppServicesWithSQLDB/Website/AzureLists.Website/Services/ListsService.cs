using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AzureLists.Website.Services
{
    public class ListsService
    {
        private string apiUri = "";
        private string ListsResource
        {
            get
            {
                return apiUri + "/api/lists";
            }
        }

        public ListsService(string apiUri)
        {
            this.apiUri = apiUri;
        }



        public async Task<List<Models.Api.List>> GetAllLists()
        {
            List<Models.Api.List> lst = new List<Models.Api.List>();

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync(ListsResource);
                if (response.IsSuccessStatusCode)
                {
                    var stringContent = await response.Content.ReadAsStringAsync();
                    lst = JsonConvert.DeserializeObject<List<Models.Api.List>>(stringContent);
                }
            }
            return ReOrder(lst);
        }

        public async Task<bool> CreateList(Models.Api.List list)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var json = JsonConvert.SerializeObject(list);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(ListsResource, content);
                if (response.IsSuccessStatusCode)
                {

                }
                return response.IsSuccessStatusCode;
            }
        }

   

        public async Task<bool> UpdateList(Models.Api.List list)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var json = JsonConvert.SerializeObject(list);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PutAsync(ListsResource, content);
                return response.IsSuccessStatusCode;
            }
        }

        private List<Models.Api.List> ReOrder(List<Models.Api.List> lst)
        {
            var inbox = lst.FirstOrDefault(x => x.Name.ToLower().Trim() == "inbox");
            var thisWeek = lst.FirstOrDefault(x => x.Name.ToLower().Trim() == "this week");
            var important = lst.FirstOrDefault(x => x.Name.ToLower().Trim() == "important");

            if (inbox != null) 
                lst.Remove(inbox);
            if (thisWeek != null)
                lst.Remove(thisWeek);
            if (important != null)
                lst.Remove(important);

            if (thisWeek != null)
                lst.Insert(0, thisWeek);
            if (important != null)
                lst.Insert(0, important);
            if (inbox != null)
                lst.Insert(0, inbox);

            return lst;
        }
    }
}