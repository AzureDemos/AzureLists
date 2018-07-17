using AzureLists.Library;
using AzureLists.TableStorage;
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

            //Remove any existing lists in the database for all users
            var lists = await service.Get<List>(user1);
            foreach (var l in lists)
                await service.Delete<List>(user1, l.Id);

            lists = await service.Get<List>(user2);
            foreach (var l in lists)
                await service.Delete<List>(user2, l.Id);

            lists = await service.Get<List>(user3);
            foreach (var l in lists)
                await service.Delete<List>(user3, l.Id);

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

            //query the database to retreive the new lists 
            lists = await service.Get<List>(user1);
            Console.WriteLine($"User 1 Lists");
            foreach (var l in lists)
            {
                Console.WriteLine($"{l.Name} loaded with {l.Tasks.Count} tasks ({l.Tasks.Where(x => x.CompletedDate == null).Count()} not completed) ");
                foreach(var t in l.Tasks.Where(x => x.CompletedDate == null))
                    Console.WriteLine($"    - {t.Title}");
            }
            Console.WriteLine("");
            lists = await service.Get<List>(user2);
            Console.WriteLine($"User 2 Lists");
            foreach (var l in lists)
            {
                Console.WriteLine($"{l.Name} loaded with {l.Tasks.Count} tasks ({l.Tasks.Where(x => x.CompletedDate == null).Count()} not completed) ");
                foreach (var t in l.Tasks.Where(x => x.CompletedDate == null))
                    Console.WriteLine($"    - {t.Title}");
            }
            Console.WriteLine("");
            lists = await service.Get<List>(user3);
            Console.WriteLine($"User 3 Lists");
            foreach (var l in lists)
            {
                Console.WriteLine($"{l.Name} loaded with {l.Tasks.Count} tasks ({l.Tasks.Where(x => x.CompletedDate == null).Count()} not completed) ");
                foreach (var t in l.Tasks.Where(x => x.CompletedDate == null))
                    Console.WriteLine($"    - {t.Title}");
            }
        }


    }
}
