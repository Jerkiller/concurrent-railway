
namespace Concurrency
{
    public static class Railway
    {

        private static (City Place, TimeSpan DistanceToTheNext)[] paths =
        {
            (City.Venezia, TimeSpan.FromMilliseconds(3_000)),
            (City.Mestre, TimeSpan.FromMilliseconds(7_000)),
            (City.Padova, TimeSpan.FromMilliseconds(4_000)),
            (City.Vicenza, TimeSpan.FromMilliseconds(2_000)),
            (City.Verona, TimeSpan.FromMilliseconds(16_000)),
        };

        private static readonly Station[] stations = [
            new (City.Venezia),
            new (City.Mestre),
            new (City.Padova),
            new (City.Vicenza),
            new (City.Verona),
        ];

        public static (City nextCity, TimeSpan travelTime) GetNextCityInfo(City city)
        {
            var currentPath = paths.First(p => p.Place == city);
            var currentIndex = Array.IndexOf(paths, currentPath);
            var (Place, DistanceToTheNext) = paths.ElementAt((currentIndex + 1) % paths.Length);
            return (Place, currentPath.DistanceToTheNext);
        }

        public static Station? GetStation(City city) => city switch
        {
            City.Verona => stations[4],
            City.Vicenza => stations[3],
            City.Padova => stations[2],
            City.Mestre => stations[1],
            City.Venezia => stations[0],
            _ => null,
        };
    }
}
