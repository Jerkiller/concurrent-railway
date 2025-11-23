using Microsoft.Extensions.Logging;

namespace Concurrency
{
    public class Program
    {
        private static readonly ILogger log = Logger.CreateLogger(nameof(Program));
        private static CancellationTokenSource cts = new CancellationTokenSource();
        public static async Task Main(string[] args)
        {

            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                log.LogInformation("Cancel event triggered");
                cts.Cancel();
                eventArgs.Cancel = true;
            };

            await RailwaySimulation(cts.Token);

            log.LogInformation("Now shutting down");
            await Task.Delay(1000);

        }

        public static async Task RailwaySimulation(CancellationToken token)
        {

            log.LogInformation("Railway simulation");

            Traveler Easy = new(1, City.Verona, City.Venezia, TimeSpan.FromMilliseconds(10_000));
            Traveler Littlemore = new(2, City.Vicenza, City.Padova, TimeSpan.FromMilliseconds(11_000));
            Traveler Normal = new(3, City.Padova, City.Verona, TimeSpan.FromMilliseconds(12_000));
            Traveler Hard = new(4, City.Venezia, City.Vicenza, TimeSpan.FromMilliseconds(13_000));
            Traveler Harder = new(5, City.Padova, City.Verona, TimeSpan.FromMilliseconds(14_000));
            Traveler Evenharder = new(6, City.Verona, City.Mestre, TimeSpan.FromMilliseconds(15_000));
            Traveler Mostofall = new(7, City.Mestre, City.Vicenza, TimeSpan.FromMilliseconds(16_000));
            Traveler Goofy = new(8, City.Padova, City.Verona, TimeSpan.FromMilliseconds(9_000));

            Task[] tasks = [
                // Passengers
                Task.Run(() => Easy.StartMiserableLife(token), token),
                Task.Run(() => Littlemore.StartMiserableLife(token), token),
                Task.Run(() => Normal.StartMiserableLife(token), token),
                Task.Run(() => Hard.StartMiserableLife(token), token),
                Task.Run(() => Harder.StartMiserableLife(token), token),
                Task.Run(() => Evenharder.StartMiserableLife(token), token),
                Task.Run(() => Mostofall.StartMiserableLife(token), token),
                Task.Run(() => Goofy.StartMiserableLife(token), token),
                // Train
                Task.Run(() => Train.StartTheJourney(token), token),
            ];

            await Task.WhenAll(tasks);
        }
    }
}