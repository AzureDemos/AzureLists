using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureLists.Library
{
    public class DummyDataGenerator
    {
        public List CreateInboxList()
        {
            Random rnd = new Random();
            var inbox = new List() { Name = "Inbox" };
            inbox.Tasks.Add(new Task() { Title = "Signup to gym", Important = true });
            inbox.Tasks.Add(new Task() { Title = "Create presentation on Azure"});
            inbox.Tasks.Add(new Task() { Title = "Pickup Tom up from airport", Important = true, DueDate= DateTime.Now.AddDays(3) });
            inbox.Tasks.Add(new Task() { Title = "Organise football fundraiser", Important = true, DueDate = DateTime.Now.AddDays(9) });
            //Complete
            inbox.Tasks.Add(new Task(){ Title = "Get invitiations printed", DueDate = DateTime.Now.AddDays(4), CompletedDate= DateTime.Now.AddDays(rnd.Next(1, 25) * 2 - 1) });
            inbox.Tasks.Add(new Task() { Title = "Find a local plumber for bathroom",  CompletedDate = DateTime.Now.AddDays(rnd.Next(1, 25) * 2 - 1) });
            inbox.Tasks.Add(new Task() { Title = "Book restaurant for Tuesday Joes birthday", DueDate = DateTime.Now.AddDays(15), CompletedDate = DateTime.Now.AddDays(rnd.Next(1, 25) * 2 - 1) });

            return inbox;
        }

        public List CreateGroceriesList()
        {
            Random rnd = new Random();
            var groceries = new List() { Name = "Groceries" };
            groceries.Tasks.Add(new Task() { Title = "Milk" });
            groceries.Tasks.Add(new Task() { Title = "Tea & Coffee" });
            groceries.Tasks.Add(new Task() { Title = "White Bread"});
            groceries.Tasks.Add(new Task() { Title = "Salmon" });
            groceries.Tasks.Add(new Task() { Title = "Eggs" });
            groceries.Tasks.Add(new Task() { Title = "New Potatoes" });
            groceries.Tasks.Add(new Task() { Title = "Beers" });
            //Complete
            groceries.Tasks.Add(new Task() { Title = "Pasta", CompletedDate = DateTime.Now.AddDays(rnd.Next(1, 25) * 2 - 1) } );
            groceries.Tasks.Add(new Task() { Title = "Tomatoes", CompletedDate = DateTime.Now.AddDays(rnd.Next(1, 25) * 2 - 1) });
            groceries.Tasks.Add(new Task() { Title = "Onions", CompletedDate = DateTime.Now.AddDays(rnd.Next(1, 25) * 2 - 1) });
            groceries.Tasks.Add(new Task() { Title = "Croissants", CompletedDate = DateTime.Now.AddDays(rnd.Next(1, 25) * 2 - 1) });
            groceries.Tasks.Add(new Task() { Title = "Black Pepper", CompletedDate = DateTime.Now.AddDays(rnd.Next(1, 25) * 2 - 1) });
            groceries.Tasks.Add(new Task() { Title = "Chicken Breasts", CompletedDate = DateTime.Now.AddDays(rnd.Next(1, 25) * 2 - 1) });
            groceries.Tasks.Add(new Task() { Title = "Tuna Steak", CompletedDate = DateTime.Now.AddDays(rnd.Next(1, 25) * 2 - 1) });

            return groceries;
        }

        public List CreateHolidayList()
        {
            Random rnd = new Random();
            var holiday = new List() { Name = "Holiday" };
            holiday.Tasks.Add(new Task() { Title = "Renew passport", Important = true, DueDate = DateTime.Now.AddDays(2)});
            holiday.Tasks.Add(new Task() { Title = "Buy new suitcase" });
            holiday.Tasks.Add(new Task() { Title = "Exchange Euros" });
            holiday.Tasks.Add(new Task() { Title = "Get new swimming shorts" });
            //Complete
            holiday.Tasks.Add(new Task() { Title = "Book Flights", Important=true, CompletedDate = DateTime.Now.AddDays(rnd.Next(1, 25) * 2 - 1) });
            holiday.Tasks.Add(new Task() { Title = "Find hotels in Spain", CompletedDate = DateTime.Now.AddDays(rnd.Next(1, 25) * 2 - 1) });
            holiday.Tasks.Add(new Task() { Title = "Research holiday destinations for end of May",CompletedDate = DateTime.Now.AddDays(rnd.Next(1, 25) * 2 - 1) });

            return holiday;
        }
    }
}
