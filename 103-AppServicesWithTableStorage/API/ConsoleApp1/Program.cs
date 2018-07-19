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
            var user1 = UserCredentials.GetFirstUserFirstPartion();
            var user2 = UserCredentials.GetSecondtUserFirstPartion();
            var user3 = UserCredentials.GetFirstUserSecondPartion();
            TableStorageListService service = new TableStorageListService();
            List<UserCredentials> users = new List<UserCredentials>() { user1, user2, user3 };


            await CleanDatabase(service, users);

            await CreateNewLists(service, user1, user2, user3);

            await QueryListsByUser(service, users);

            await GetInvidualListAndPerformUpdate(service, user1); //get list, change name, add task, re-load and check

            await GetInvidualTaskAndPerformUpdate(service, user1); // get task, change title and important, re-load and check

            await SearchTasksByImportantOrCompleted(service, user2);

            await DeleteTask(service, user3);
        }

        static async System.Threading.Tasks.Task CleanDatabase(TableStorageListService service, List<UserCredentials> users)
        {
            Console.WriteLine("********* Cleaning Database *********\r\n");
            foreach (var user in users)
            {
                var lists = await service.Get<List>(user);
                foreach (var l in lists)
                    await service.Delete<List>(user, l.Id);
            }
        }

        static async System.Threading.Tasks.Task CreateNewLists(TableStorageListService service, UserCredentials user1, UserCredentials user2, UserCredentials user3)
        {
            Console.WriteLine("********* Creating new Lists *********\r\n");
            //generate some demo lists
            var dummyData = new DummyDataGenerator();
            var inbox = dummyData.CreateInboxList();
            var groceries = dummyData.CreateGroceriesList();
            var holiday = dummyData.CreateHolidayList();

            //populate the database with the new lists
            await service.Create<List>(user1, inbox);
            await service.Create<List>(user1, groceries);
            await service.Create<List>(user2, holiday); //user 2
            await service.Create<List>(user3, inbox); // user 3 which is in partition 2
            Console.WriteLine("4 Lists created across 3 users in 2 partitions");
        }

        static async System.Threading.Tasks.Task QueryListsByUser(TableStorageListService service, List<UserCredentials> users)
        {
            Console.WriteLine("\r\n********* Getting Lists by User *********\r\n");
            foreach (var user in users)
            {
                var lists = await service.Get<List>(user);
                Console.WriteLine($"User {user.UserID} Lists");
                foreach (var l in lists)
                {
                    Console.WriteLine($"  {l.Name} loaded with {l.Tasks.Count} tasks ({l.Tasks.Where(x => x.CompletedDate == null).Count()} not completed) ");
                    foreach (var t in l.Tasks.Where(x => x.CompletedDate == null))
                        Console.WriteLine($"    - {t.Title}");
                }
                Console.WriteLine("");
            }
        }

        static async System.Threading.Tasks.Task GetInvidualListAndPerformUpdate(TableStorageListService service, UserCredentials user)
        {
            Console.WriteLine("\r\n********* Updating a List *********\r\n");
            //Get Individual List
            Console.WriteLine("");
            Console.WriteLine($"Loading inbox list from user 3");
            Console.WriteLine("");
            var lists = await service.Get<List>(user);
            var listId = lists.ToList()[0].Id;
            var list = await service.Get<List>(user, listId);

            Console.WriteLine($"LOADED {list.Name} with {list.Tasks.Count} tasks ({list.Tasks.Where(x => x.CompletedDate == null).Count()} not completed) {list.Id} for user {user.UserID}");
            //Update List
            list.Name = "Inbox-NewName";
            list.Tasks.Add(new AzureLists.Library.Task() { Title = "New-Task" });
            list = await service.Replace<List>(user, list.Id, list);
            Console.WriteLine($"UPDATED {list.Name} with {list.Tasks.Count} tasks ({list.Tasks.Where(x => x.CompletedDate == null).Count()} not completed) {list.Id} for user {user.UserID}");

            //Re-Read Updated List
            list = await service.Get<List>(user, listId);
            Console.WriteLine($"RE-LOADED {list.Name} with {list.Tasks.Count} tasks ({list.Tasks.Where(x => x.CompletedDate == null).Count()} not completed) {list.Id} for user {user.UserID}");

        }

        static async System.Threading.Tasks.Task GetInvidualTaskAndPerformUpdate(TableStorageListService service, UserCredentials user)
        {
            Console.WriteLine("\r\n********* Updateing a Task *********\r\n");
            var lists = await service.Get<List>(user);
            //Get Individual Task
            var taskId = lists.ToList()[0].Tasks[0].Id;
            var task = await service.Get<AzureLists.Library.Task>(user, taskId);
            Console.WriteLine($"LOADED task - {task.Title} (Important: {task.Important}) for user {user.UserID}");

            //Update Task
            task.Title += " and another thing";
            task.Important = !task.Important;
            task = await service.Replace<AzureLists.Library.Task>(user, taskId, task);
            Console.WriteLine($"UPDATED task - {task.Title} (Important: {task.Important}) for user {user.UserID}");

            //Re-Read Updated List
            task = await service.Get<AzureLists.Library.Task>(user, taskId);
            Console.WriteLine($"RE-LOADED task - {task.Title} (Important: {task.Important}) for user {user.UserID}");

            Console.WriteLine("");
        }

        static async System.Threading.Tasks.Task DeleteTask(TableStorageListService service, UserCredentials user)
        {
            Console.WriteLine("\r\n********* Deleting a Task *********\r\n");
            var lists = await service.Get<List>(user);
            var list = lists.ToList()[0];
            Console.WriteLine($"List {list.Name} has {list.Tasks.Count} tasks for user {user.UserID}");
            //Delete Individual Task
            Console.WriteLine("Deleting a task");
            await service.Delete<AzureLists.Library.Task>(user, list.Tasks[0].Id);
            list = await service.Get<List>(user, list.Id);
            Console.WriteLine($"List {list.Name} has {list.Tasks.Count} tasks for user {user.UserID}");
            Console.WriteLine("");
        }

        static async System.Threading.Tasks.Task SearchTasksByImportantOrCompleted(TableStorageListService service, UserCredentials user)
        {
            Console.WriteLine("\r\n********* Searching Tasks *********\r\n");
            var importantTasks = await service.Get<AzureLists.Library.Task>(user,important:true);
            Console.WriteLine($"{importantTasks.Count()} Important tasks found for user {user.UserID}");

            var completedTasks = await service.Get<AzureLists.Library.Task>(user, completed: true);
            Console.WriteLine($"{completedTasks.Count()} Completed tasks found for user {user.UserID}");

            Console.WriteLine("");
        }
    }
}
