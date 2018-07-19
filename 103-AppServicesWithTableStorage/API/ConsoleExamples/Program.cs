using AzureLists.Library;
using AzureLists.TableStorage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            System.Threading.Tasks.Task t = MainAsync();
            t.Wait();
            Console.ReadLine();
        }

        static async System.Threading.Tasks.Task MainAsync()
        {
            //Not using TableStorageListService as IListService becuase we want to access property: service.listRepository.AuthenticatedUser.UserID (just for console logging purposes)
            //In the IOC TableStorageListService is defined as IListService, which is what the API controllers receive 
            TableStorageListService serviceForUser1 = new TableStorageListService(new TableStorageRepository(UserCredentials.GetFirstUserFirstPartion()));
            TableStorageListService serviceForUser2 = new TableStorageListService(new TableStorageRepository(UserCredentials.GetSecondtUserFirstPartion()));
            TableStorageListService serviceForUser3 = new TableStorageListService(new TableStorageRepository(UserCredentials.GetFirstUserSecondPartion()));
            List<TableStorageListService> services = new List<TableStorageListService>() { serviceForUser1, serviceForUser2, serviceForUser3 };


            await CleanDatabase(services);

            await CreateNewLists(serviceForUser1, serviceForUser2, serviceForUser3);

            await QueryListsByUser(services);

            await GetInvidualListAndPerformUpdate(serviceForUser1); //get list, change name, add task, re-load and check

            await GetInvidualTaskAndPerformUpdate(serviceForUser1); // get task, change title and important, re-load and check

            await SearchTasksByImportantOrCompleted(serviceForUser2);

            await DeleteTask(serviceForUser3);
        }

        static async System.Threading.Tasks.Task CleanDatabase(List<TableStorageListService> services)
        {
            Console.WriteLine("********* Cleaning Database *********\r\n");
            foreach (var service in services)
            {
                var lists = await service.GetAllLists();
                foreach (var l in lists)
                    await service.DeleteList(l.Id);
            }
        }

        static async System.Threading.Tasks.Task CreateNewLists(TableStorageListService serviceForUser1, TableStorageListService serviceForUser2, TableStorageListService serviceForUser3)
        {
            Console.WriteLine("********* Creating new Lists *********\r\n");
            //generate some demo lists
            var dummyData = new DummyDataGenerator();
            var inbox = dummyData.CreateInboxList();
            var groceries = dummyData.CreateGroceriesList();
            var holiday = dummyData.CreateHolidayList();

            //populate the database with the new lists
            await serviceForUser1.CreateOrUpdateList(inbox);
            await serviceForUser1.CreateOrUpdateList(groceries);
            await serviceForUser2.CreateOrUpdateList(holiday); //user 2
            await serviceForUser3.CreateOrUpdateList(inbox); // user 3 which is in partition 2
            Console.WriteLine("4 Lists created across 3 users in 2 partitions");
        }

        static async System.Threading.Tasks.Task QueryListsByUser(List<TableStorageListService> services)
        {
            Console.WriteLine("\r\n********* Getting Lists by User *********\r\n");
            foreach (var service in services)
            {
                var lists = await service.GetAllLists();
                Console.WriteLine($"User {service.listRepository.AuthenticatedUser.UserID} Lists");
                foreach (var l in lists)
                {
                    Console.WriteLine($"  {l.Name} loaded with {l.Tasks.Count} tasks ({l.Tasks.Where(x => x.CompletedDate == null).Count()} not completed) ");
                    foreach (var t in l.Tasks.Where(x => x.CompletedDate == null))
                        Console.WriteLine($"    - {t.Title}");
                }
                Console.WriteLine("");
            }
        }

        static async System.Threading.Tasks.Task GetInvidualListAndPerformUpdate(TableStorageListService service)
        {
            Console.WriteLine("\r\n********* Updating a List *********\r\n");
            //Get Individual List
            Console.WriteLine("");
            Console.WriteLine($"Loading inbox list from user 3");
            Console.WriteLine("");
            var lists = await service.GetAllLists();
            var listId = lists.ToList()[0].Id;
            var list = await service.GetListById(listId);

            Console.WriteLine($"LOADED {list.Name} with {list.Tasks.Count} tasks ({list.Tasks.Where(x => x.CompletedDate == null).Count()} not completed) {list.Id} for user {service.listRepository.AuthenticatedUser.UserID}");
            //Update List
            list.Name = "Inbox-NewName";
            list.Tasks.Add(new AzureLists.Library.Task() { Title = "New-Task" });
            list = await service.CreateOrUpdateList(list);
            Console.WriteLine($"UPDATED {list.Name} with {list.Tasks.Count} tasks ({list.Tasks.Where(x => x.CompletedDate == null).Count()} not completed) {list.Id} for user {service.listRepository.AuthenticatedUser.UserID}");

            //Re-Read Updated List
            list = await service.GetListById(listId);
            Console.WriteLine($"RE-LOADED {list.Name} with {list.Tasks.Count} tasks ({list.Tasks.Where(x => x.CompletedDate == null).Count()} not completed) {list.Id} for user {service.listRepository.AuthenticatedUser.UserID}");

        }

        static async System.Threading.Tasks.Task GetInvidualTaskAndPerformUpdate(TableStorageListService service)
        {
            Console.WriteLine("\r\n********* Updateing a Task *********\r\n");
            var lists = await service.GetAllLists();
            //Get Individual Task
            var taskId = lists.ToList()[0].Tasks[0].Id;
            var task = await service.GetTaskById(taskId);
            Console.WriteLine($"LOADED task - {task.Title} (Important: {task.Important}) for user {service.listRepository.AuthenticatedUser.UserID}");

            //Update Task
            task.Title += " and another thing";
            task.Important = !task.Important;
            task = await service.ReplaceTask(task);
            Console.WriteLine($"UPDATED task - {task.Title} (Important: {task.Important}) for user {service.listRepository.AuthenticatedUser.UserID}");

            //Re-Read Updated List
            task = await service.GetTaskById(taskId);
            Console.WriteLine($"RE-LOADED task - {task.Title} (Important: {task.Important}) for user {service.listRepository.AuthenticatedUser.UserID}");

            Console.WriteLine("");
        }

        static async System.Threading.Tasks.Task DeleteTask(TableStorageListService service)
        {
            Console.WriteLine("\r\n********* Deleting a Task *********\r\n");
            var lists = await service.GetAllLists();
            var list = lists.ToList()[0];
            Console.WriteLine($"List {list.Name} has {list.Tasks.Count} tasks for user {service.listRepository.AuthenticatedUser.UserID}");
            //Delete Individual Task
            Console.WriteLine("Deleting a task");
            await service.DeleteTask(list.Tasks[0].Id);
            list = await service.GetListById(list.Id);
            Console.WriteLine($"List {list.Name} has {list.Tasks.Count} tasks for user {service.listRepository.AuthenticatedUser.UserID}");
            Console.WriteLine("");
        }

        static async System.Threading.Tasks.Task SearchTasksByImportantOrCompleted(TableStorageListService service)
        {
            Console.WriteLine("\r\n********* Searching Tasks *********\r\n");
            var importantTasks = await service.SearchTasks(important: true);
            Console.WriteLine($"{importantTasks.Count()} Important tasks found for user {service.listRepository.AuthenticatedUser.UserID}");

            var completedTasks = await service.SearchTasks(completed: true);
            Console.WriteLine($"{completedTasks.Count()} Completed tasks found for user {service.listRepository.AuthenticatedUser.UserID}");

            Console.WriteLine("");
        }
    }
}
