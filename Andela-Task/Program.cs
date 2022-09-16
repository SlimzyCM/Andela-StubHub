namespace Andela_Task
{
    public class Event
    {
        public string Name { get; set; }
        public string City { get; set; }
    }
    public class Customer
    {
        public string Name { get; set; }
        public string City { get; set; }
    }
    public class Program
    {
        private static readonly Dictionary<string, int> QueryStore = new();
        static void Main(string[] args)
        {
            var events = new List<Event>{
                new Event{ Name = "Phantom of the Opera", City = "New York"},
                new Event{ Name = "Metallica", City = "Los Angeles"},
                new Event{ Name = "Metallica", City = "New York"},
                new Event{ Name = "Metallica", City = "Boston"},
                new Event{ Name = "LadyGaGa", City = "New York"},
                new Event{ Name = "LadyGaGa", City = "Boston"},
                new Event{ Name = "LadyGaGa", City = "Chicago"},
                new Event{ Name = "LadyGaGa", City = "San Francisco"},
                new Event{ Name = "LadyGaGa", City = "Washington"}
            };
                /*
                var customers = new List<Customer>{
                new Customer{ Name = "Nathan", City = "New York"},
                new Customer{ Name = "Bob", City = "Boston"},
                new Customer{ Name = "Cindy", City = "Chicago"},
                new Customer{ Name = "Lisa", City = "Los Angeles"}
                };
               */
            //1. find out all events that are in cities of customer
            // then add to email.
            var customer = new Customer { Name = "Mr. Fake", City = "New York" };


            // 1. TASK --------------------------------------------------------------------------------------
            /*
            * We want you to send an email to this customer with all events in their city
            * Just call AddToEmail(customer, event) for each event you think they should get
            */
            //Simplify the given query
            var query = events.Where(e => e.City.Contains(customer.City)).ToList();
            //Send Email
            query.ForEach(e => AddToEmail(customer, e));


            // 2. TASK ---------------------------------------------------------------------------------------
            //get the first 5 closest cities
            const int numbersRequired = 5;
            var closestCities = events
                .OrderBy(e => GetDistance(customer.City, e.City))
                .Take(numbersRequired)
                .ToList();
            //send them to client in an email
            closestCities.ForEach(e => AddToEmail(customer, e));


            // 3. TASK -----------------------------------------------------------------------------------------
            //The way I think I could improve the code written in two is to store the closest city result in a cache, and reuse it 
            // I will introduce a method to get from cache first before calling the GetDistance method and also add to my cache if not found
            var improvedClosestCities = events
                .OrderBy(e => HandleGetDistance(customer.City, e.City))
                .Take(numbersRequired)
                .ToList();
            //send email
            improvedClosestCities.ForEach(e => AddToEmail(customer, e));


            // 4. TASK ------------------------------------------------------------------------------------------
            // If the GetDistance can fail, that means we are bound to have exceptions, I will introduce exception handling to the HandleGetDistance method
            // while still using the cache implementation
            var errorImprovedClosestCities = events
                .OrderBy(e => HandleGetDistanceWithError(customer.City, e.City))
                .Take(numbersRequired)
                .ToList();
            //send email
            errorImprovedClosestCities.ForEach(e => AddToEmail(customer, e));


            // 5. TASK ------------------------------------------------------------------------------------------
            // To sort the events also by other fields like price, I'll use a ThenBy Linq method
            var closeCityEvents = events
                .OrderBy(e => HandleGetDistanceWithError(customer.City, e.City))
                .ThenBy(GetPrice) //shorten .ThenBy(e => GetPrice(e)) using method group
                .ToList();
            //send email
        }
        // You do not need to know how these methods work
        static void AddToEmail(Customer c, Event e, int? price = null)
        {
            var distance = GetDistance(c.City, e.City);
            Console.Out.WriteLine($"{c.Name}: {e.Name} in {e.City}"
            + (distance > 0 ? $" ({distance} miles away)" : "")
            + (price.HasValue ? $" for ${price}" : ""));
        }
        static int GetPrice(Event e)
        {
            return (AlphebiticalDistance(e.City, "") + AlphebiticalDistance(e.Name, "")) / 10;
        }

        private static int HandleGetDistance(string start, string end)
        {
            var searchKey = start + end;
            if (start == end) return 0;

            if (QueryStore.ContainsKey(searchKey))
            {
                var res = QueryStore.FirstOrDefault(q => q.Key == searchKey);
                return res.Value;
            }

            var newDistance = GetDistance(start, end);
            QueryStore.Add(searchKey, newDistance);
            return newDistance;
        }

        private static int HandleGetDistanceWithError(string start, string end)
        {
            try
            {
                var searchKey = start + end;
                if (start == end) return 0;

                if (QueryStore.ContainsKey(searchKey))
                {
                    var res = QueryStore.FirstOrDefault(q => q.Key == searchKey);
                    return res.Value;
                }

                var newDistance = GetDistance(start, end);
                QueryStore.Add(searchKey, newDistance);
                return newDistance;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 0;
            }
        }

        static int GetDistance(string fromCity, string toCity)
        {
            return AlphebiticalDistance(fromCity, toCity);
        }
        private static int AlphebiticalDistance(string s, string t)
        {
            var result = 0;
            var i = 0;
            for (i = 0; i < Math.Min(s.Length, t.Length); i++)
            {
                // Console.Out.WriteLine($"loop 1 i={i} {s.Length} {t.Length}");
                result += Math.Abs(s[i] - t[i]);
            }
            for (; i < Math.Max(s.Length, t.Length); i++)
            {
                // Console.Out.WriteLine($"loop 2 i={i} {s.Length} {t.Length}");
                result += s.Length > t.Length ? s[i] : t[i];
            }
            return result;
        }
    }
}
