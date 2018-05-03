using AzureLists.Website.Extensions;
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

        public async Task<Models.Api.List> GetList(string Id)
        {
            Models.Api.List lst = null;

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync(ListsResource + $"/{Id}");
                if (response.IsSuccessStatusCode)
                {
                    var stringContent = await response.Content.ReadAsStringAsync();
                    lst = JsonConvert.DeserializeObject<Models.Api.List>(stringContent);
                }
            }
            return lst;
        }

        public async Task<(bool success, string newListId)> CreateList(Models.Api.List list)
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
                    var responseString = await response.Content.ReadAsStringAsync();
                    var outputList = JsonConvert.DeserializeObject<Models.Api.List>(responseString);
                    return (success: true, newListId: outputList.Id);
                }
                else
                    return (success: false, newListId:"");
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
                HttpResponseMessage response = await client.PutAsync(ListsResource + $"/{list.Id}", content);
                return response.IsSuccessStatusCode;
            }
        }


        #region Tasks

        public async Task<Models.Api.Task> GetTask(string listId, string taskId)
        {
            Models.Api.Task tsk = null;

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync(ListsResource + $"/{listId}/tasks/{taskId}");
                if (response.IsSuccessStatusCode)
                {
                    var stringContent = await response.Content.ReadAsStringAsync();
                    tsk = JsonConvert.DeserializeObject<Models.Api.Task>(stringContent);
                }
            }
            return tsk;
        }

        public async Task<(bool success, string newTaskId)> AddTaskToList(string listId, Models.Api.Task task)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var json = JsonConvert.SerializeObject(task);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(ListsResource + $"/{listId}/tasks", content);
                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var outputList = JsonConvert.DeserializeObject<Models.Api.Task>(responseString);
                    return (success: true, newTaskId: outputList.Id);
                }
                else
                    return (success: false, newTaskId: "");
            }
        }

        public async Task<bool> UpdateTask(string listId, Models.Api.Task task)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var json = JsonConvert.SerializeObject(task);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PutAsync(ListsResource + $"/{listId}/tasks/{task.Id}", content);
                return response.IsSuccessStatusCode;
            }
        }

        #endregion

        private List<Models.Api.List> ReOrder(List<Models.Api.List> lst)
        {
            var inbox = lst.FirstOrDefault(x => x.Name.ToLower().Trim() == "inbox");
            List<Models.Api.Task> allTasks = new List<Models.Api.Task>();
            foreach (var l in lst)
            {
                foreach (var t in l.Tasks)
                    t.ListId = l.Id;
                allTasks.AddRange(l.Tasks);
            }
                

            if (inbox != null) 
                lst.Remove(inbox);

            lst.Insert(0, new Models.Api.List() { Name = "This Week", Tasks = allTasks.Where(x => x.DueDate.HasValue && x.DueDate.Value <= DateTime.Now.ToStartOfDay().AddDays(7)).ToList() });
            lst.Insert(0, new Models.Api.List() { Name = "Important", Tasks = allTasks.Where(x => x.Important).ToList() });
            if (inbox != null)
                lst.Insert(0, inbox);

            return lst;
        }
    }
}