
namespace Concurrency
{
    public static class Railway
    {

        public static readonly (City Place, TimeSpan DistanceToTheNext)[] paths =
        {
            (City.Verona, TimeSpan.FromMilliseconds(3_000)),
            (City.Vicenza, TimeSpan.FromMilliseconds(7_000)),
            (City.Padova, TimeSpan.FromMilliseconds(4_000)),
            (City.Mestre, TimeSpan.FromMilliseconds(2_000)),
            (City.Venezia, TimeSpan.FromMilliseconds(16_000)),
        };

        private static readonly Station[] stations = [
            new (City.Verona),
            new (City.Vicenza),
            new (City.Padova),
            new (City.Mestre),
            new (City.Venezia),
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
            City.Verona => stations[0],
            City.Vicenza => stations[1],
            City.Padova => stations[2],
            City.Mestre => stations[3],
            City.Venezia => stations[4],
            _ => null,
        };
    }
}
