using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task StartMiserableLife()
        {
            log.LogInformation("Traveler {traveler} started their miserable life.", Id);
            while (true)
            {
                await Task.Delay(100);
                City destination = GetDestination();
                Station? nearestStation = GetNearestStation();
                await nearestStation.Transfer(destination, this);
                await Task.Delay(WaitTime);
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
