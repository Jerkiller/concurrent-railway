using Microsoft.Extensions.Logging;

namespace Concurrency
{
    public enum Direction { ToWork, BackHome }
    public class Traveler(int id, City home, City work, TimeSpan waitTime)
    {
        public readonly int Id = id;
        public readonly City Home = home;
        public readonly City Work = work;
        public readonly TimeSpan WaitTime = waitTime;
        private static readonly ILogger log = Logger.CreateLogger(nameof(Traveler));
        private Direction currentDirection = Direction.ToWork;

        public async Task StartMiserableLife(CancellationToken token)
        {
            log.LogInformation("Traveler {traveler} started their miserable life.", Id);
            while (!token.IsCancellationRequested)
            {
                //await Task.Delay(100, token);
                City destination = GetDestination();
                Station? nearestStation = GetNearestStation();
                nearestStation.Transfer(destination, this);
                await Task.Delay(WaitTime, token);
                SwapDirections();
            }
        }

        public City GetDestination() => currentDirection == Direction.ToWork ? Work : Home;

        private Station GetNearestStation() => 
            Railway.GetStation(currentDirection == Direction.ToWork ? Home : Work)
                ?? throw new InvalidOperationException("Station not found.");

        private void SwapDirections()
        {
            currentDirection = currentDirection == Direction.ToWork
                ? Direction.BackHome
                : Direction.ToWork;
        }
    }
}
