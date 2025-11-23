using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Concurrency
{
    public static class Train
    {
        public static City currentCity = City.Venezia;
        private static readonly ILogger log = Logger.CreateLogger(nameof(Train));
        private static readonly List<Traveler> peopleOnBoard = new List<Traveler>();
        public const int totalTrainCapacity = 4;

        public static string Aboard()
        {
            var peopleString = string.Join("|", peopleOnBoard.Select(p => p.Id));
            var emptySeats = totalTrainCapacity - peopleOnBoard.Count;
            peopleString += 0 < emptySeats && emptySeats < 4 ? "|" : "";
            peopleString += string.Join("|", Enumerable.Repeat("_", emptySeats).ToArray());
            return "|" + peopleString + "|";
        }

        public static void LeaveTheTrain(Traveler traveler)
        {
            if (peopleOnBoard.Count == 0)
            {
                throw new InvalidOperationException("Train is empty and someone is trying to leave.");
            }

            peopleOnBoard.Remove(traveler);
            log.LogInformation("{aboard}\t\t#{travelerId} has left", Aboard(), traveler.Id);
        }


        public static void EnterTheTrain(Traveler traveler)
        {
            if (peopleOnBoard.Count >= totalTrainCapacity)
            {
                throw new InvalidOperationException("Train is at full capacity");
            }

            peopleOnBoard.Add(traveler);
            log.LogInformation("{aboard}\t\t#{travelerId} has boarded", Aboard(), traveler.Id);
        }

        public static async Task StartTheJourney()
        {
            await Task.Delay(TimeSpan.FromMilliseconds(250));
            while (true)
            {
                log.LogInformation("{aboard} ~~{currentCity}~~", Aboard(), currentCity.ToString().ToUpperInvariant());
                (City nextCity, TimeSpan travelTime) = Railway.GetNextCityInfo(currentCity);
                var station = Railway.GetStation(currentCity)
                    ?? throw new InvalidOperationException("Station not found.");

                station.Calling(peopleOnBoard.Count);
                await Task.Delay(100);
                station.Boarding();
                await Task.Delay(100);
                station.Departing(peopleOnBoard.Count);
                await Task.Delay(travelTime);
                currentCity = nextCity;
                
            }

        }

    }
}
