using Microsoft.Extensions.Logging;

namespace Concurrency
{
    public class Station(City location)
    {
        private readonly City Location = location;
        private bool trainBoarding = false;
        private bool trainStopped = false; // train at the station
        private int currentlyOnTheTrain = 0; // = transported. reliable only if trainStopped

        // counters to track how many threads are waiting to alight / to board
        private int waitingToAlight = 0;
        private int waitingToBoard = 0;

        private readonly object departureLock = new();
        private readonly object arrivalLock = new();
        // private static readonly ILogger log = Logger.CreateLogger(nameof(Station));
        public void Transfer(City destination, Traveler traveler)
        {
            lock (departureLock)
            {
                // register as a waiting-to-board traveler
                waitingToBoard++;
                while (!trainBoarding || currentlyOnTheTrain == Train.totalTrainCapacity)
                    Monitor.Wait(departureLock);
                // log.LogInformation("Traveler {travelerId} is boarding the train at {station} towards {destination}.", traveler.Id, Location, destination);
                waitingToBoard--;
                currentlyOnTheTrain++;
                Train.EnterTheTrain(traveler);
                Monitor.PulseAll(departureLock);
            }
            // alight on another station
            var station = Railway.GetStation(destination)
                ?? throw new InvalidOperationException("Station not found.");
            station.Alight(destination, traveler);
        }

        public void Alight(City destination, Traveler traveler)
        {
            lock (arrivalLock)
            {
                // waiting-to-alight traveler
                waitingToAlight++;
                // log.LogInformation("Traveler {travelerId} would like to arrive at {destination}.", traveler.Id, destination);
                while (Location != destination || !trainStopped)
                {
                    // log.LogDebug("WAIT: Traveler {travelerId} is waiting to alight at {destination}. Current station: {station}.", traveler.Id, destination, Location);
                    Monitor.Wait(arrivalLock);
                }
                // log.LogInformation("Traveler {travelerId} is alighting the train at {station}.", traveler.Id, Location);
                
                waitingToAlight--;
                currentlyOnTheTrain--;
                Train.LeaveTheTrain(traveler);
                Monitor.PulseAll(arrivalLock);
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
                // wait until no more alighters are waiting (they decrement waitingToAlight when done)
                while (waitingToAlight > 0)
                {
                    Monitor.Wait(arrivalLock);
                }
            }
        }

        public void Boarding()
        {
            lock (departureLock)
            {
                // log.LogInformation("BOARDING at {station}.", Location);
                trainStopped = false;
                trainBoarding = true;
                
                // let waiting boarders try to acquire seats
                Monitor.PulseAll(departureLock);

                while (waitingToBoard > 0 && currentlyOnTheTrain < Train.totalTrainCapacity)
                {
                    Monitor.Wait(departureLock);
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
