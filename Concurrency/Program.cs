using Concurrency;
using System.Threading.Tasks;

Console.WriteLine("Railway simulation");

Traveler Easy = new (1, City.Verona, City.Venezia, TimeSpan.FromMilliseconds(10_000));
Traveler Littlemore = new (2, City.Vicenza, City.Padova, TimeSpan.FromMilliseconds(11_000));
Traveler Normal = new (3, City.Padova, City.Verona, TimeSpan.FromMilliseconds(12_000));
Traveler Hard = new (4, City.Venezia, City.Vicenza, TimeSpan.FromMilliseconds(13_000));
Traveler Harder = new (5, City.Padova, City.Verona, TimeSpan.FromMilliseconds(14_000));
Traveler Evenharder = new (6, City.Verona, City.Mestre, TimeSpan.FromMilliseconds(15_000));
Traveler Mostofall = new (7, City.Mestre, City.Vicenza, TimeSpan.FromMilliseconds(16_000));
Traveler Goofy = new (8, City.Padova, City.Verona, TimeSpan.FromMilliseconds(9_000));

Task[] tasks = [
    Train.StartTheJourney(),
    // Passengers
    Easy.StartMiserableLife(),
    Littlemore.StartMiserableLife(),
    Normal.StartMiserableLife(),
    Hard.StartMiserableLife(),
    Harder.StartMiserableLife(),
    Evenharder.StartMiserableLife(),
    Mostofall.StartMiserableLife(),
    Goofy.StartMiserableLife(),
];

await Task.WhenAll(tasks);