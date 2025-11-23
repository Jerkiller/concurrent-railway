using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Concurrency
{
    public class Station(City location)
    {
        private readonly City Location = location;
        private bool trainBoarding = false;
        private bool trainStopped = false; // train at the station
        private int currentlyOnTheTrain = 0; // = transported. reliable only if trainStopped
        private readonly object departureLock = new();
        private readonly object arrivalLock = new();
        private static readonly ILogger log = Logger.CreateLogger(nameof(Station));
        public async Task Transfer(City destination, Traveler traveler)
        {
            lock(departureLock)
            {
                while (!trainBoarding || currentlyOnTheTrain == Train.totalTrainCapacity)
                    Monitor.Wait(departureLock);
                // log.LogInformation("Traveler {travelerId} is boarding the train at {station} towards {destination}.", traveler.Id, Location, destination);
                currentlyOnTheTrain++;
                Train.EnterTheTrain(traveler);
            }
            // alight on another station
            var station = Railway.GetStation(destination)
                ?? throw new InvalidOperationException("Station not found.");
            await station.Alight(destination, traveler);
        }

        public async Task Alight(City destination, Traveler traveler)
        {
            lock (arrivalLock)
            {
                // log.LogInformation("Traveler {travelerId} would like to arrive at {destination}.", traveler.Id, destination);
                while (Location != destination || !trainStopped)
                {
                    // log.LogDebug("WAIT: Traveler {travelerId} is waiting to alight at {destination}. Current station: {station}.", traveler.Id, destination, Location);
                    Monitor.Wait(arrivalLock);
                }
                // log.LogInformation("Traveler {travelerId} is alighting the train at {station}.", traveler.Id, Location);
                currentlyOnTheTrain--;
                Train.LeaveTheTrain(traveler);
            }
        }

        public void Calling(int load)
        {
            lock (arrivalLock)
            {
                // log.LogInformation("CALLING at {station} with {load} passengers on board.", Location, load);
                trainStopped = true;
                currentlyOnTheTrain = load;
                Monitor.PulseAll(arrivalLock);
            }
        }

        public void Boarding()
        {
            lock (departureLock)
            {
                // log.LogInformation("BOARDING at {station}.", Location);
                trainStopped = false;
                trainBoarding = true;
                if (currentlyOnTheTrain < Train.totalTrainCapacity)
                {
                    Monitor.PulseAll(departureLock);
                }
            }
        }

        public void Departing(int load)
        {
            // log.LogInformation("DEPARTING from {station} with {load} passengers on board.", Location, load);
            trainBoarding = false;
        }

    }
}
