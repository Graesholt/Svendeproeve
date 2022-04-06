using WebAPI.Models;

namespace WebAPI.DTO
{
    public class RunStats
    {
        public RunStats()
        {

        }

        public RunStats(Run run)
        {
            dateTime = run.dateTime;
            points = run.points;

            //CenterPoint
            double northMax = 0;
            double southMax = 0;
            double eastMax = 0;
            double westMax = 0;
            for (int i = 0; i < run.points.Count(); i++)
            {
                if ((run.points[i].latitude > northMax) || (northMax == 0))
                {
                    northMax = run.points[i].latitude;
                }
                if ((run.points[i].latitude < southMax) || (southMax == 0))
                {
                    southMax = run.points[i].latitude;
                }

                if ((run.points[i].longitude > eastMax) || (eastMax == 0))
                {
                    eastMax = run.points[i].longitude;
                }
                if ((run.points[i].longitude < westMax) || (westMax == 0))
                {
                    westMax = run.points[i].longitude;
                }
            }
            centerLatitude = southMax + ((northMax - southMax) / 2);
            centerLongitude = westMax + ((eastMax - westMax) / 2);

            //Distance
            distance = 0;
            for (int i = 0; i < run.points.Count() - 1; i++)
            {
                distance += latLongDistanceiInMeters(run.points[i].latitude, run.points[i].longitude, run.points[i + 1].latitude, run.points[i + 1].longitude);
            }

            //Duration
            duration = run.points.Last().dateTime.Subtract(run.dateTime);

            //Average Speed
            avgSpeedInMetersPerSecond = distance / duration.TotalSeconds;

            //Average Speed pr minute (for chart)
            avgSpeedPerMinuteInMetersPerSecond = new List<double>();
            avgSpeedPerMinuteInMetersPerSecond.Add(0);
            int minute = 1;
            int restPrecisionFactor = 100;
            for (int i1 = 0; i1 < run.points.Count() - 1; i1++)
            {
                if (run.points[i1 + 1].dateTime < run.dateTime.AddMinutes(minute))
                {
                    avgSpeedPerMinuteInMetersPerSecond[minute - 1] += latLongDistanceiInMeters(run.points[i1].latitude, run.points[i1].longitude, run.points[i1 + 1].latitude, run.points[i1 + 1].longitude);
                }
                else
                {
                    //Calculate leftover, put rest in next minute
                    avgSpeedPerMinuteInMetersPerSecond.Insert(minute, 0);

                    TimeSpan restTime = (run.points[i1 + 1].dateTime - run.points[i1].dateTime) / restPrecisionFactor;
                    double restDistance = latLongDistanceiInMeters(run.points[i1].latitude, run.points[i1].longitude, run.points[i1 + 1].latitude, run.points[i1 + 1].longitude) / restPrecisionFactor;
                    for (int i2 = 1; i2 <= restPrecisionFactor; i2++)
                    {
                        if (run.points[i1].dateTime + (i2 * restTime) < run.dateTime.AddMinutes(minute))
                        {
                            avgSpeedPerMinuteInMetersPerSecond[minute - 1] += restDistance;
                        }
                        else
                        {
                            avgSpeedPerMinuteInMetersPerSecond[minute] += restDistance;
                        }
                    }

                    minute++;
                }
            }
            for (int i = 0; i < avgSpeedPerMinuteInMetersPerSecond.Count(); i++)
            {
                if (i < avgSpeedPerMinuteInMetersPerSecond.Count() - 1)
                {
                    avgSpeedPerMinuteInMetersPerSecond[i] = avgSpeedPerMinuteInMetersPerSecond[i] / 60;
                }
                else
                {
                    //Last minute might not be a whole minute...
                    //castings to double to force decimal results.
                    avgSpeedPerMinuteInMetersPerSecond[i] = avgSpeedPerMinuteInMetersPerSecond[i] / ((double)duration.Seconds + ((double)duration.Milliseconds / (double)1000));
                }
            }
        }

        public double centerLatitude { get; set; }

        public double centerLongitude { get; set; }

        public DateTime dateTime { get; set; }

        public List<Point> points { get; set; }

        public double distance { get; set; }

        public TimeSpan duration { get; set; }

        public double avgSpeedInMetersPerSecond { get; set; }

        public List<double> avgSpeedPerMinuteInMetersPerSecond { get; set; }

        //Calculating distance from coordinates is no simple task, so the following function has been stolen with pride from stackoverflow.
        //Originally written in JavaScript, the small task of converting it to C# fell to myself.
        //It is based on the Haversine formula, a formula for calculating distance on a spherical surface, the sphere in this case being the planet.
        //https://stackoverflow.com/questions/639695/how-to-convert-latitude-or-longitude-to-meters
        static double latLongDistanceiInMeters(double lat1, double lon1, double lat2, double lon2)
        {
            double R = 6378.137; // Radius of earth in KM
            double dLat = lat2 * Math.PI / 180 - lat1 * Math.PI / 180;
            double dLon = lon2 * Math.PI / 180 - lon1 * Math.PI / 180;
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
            Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
            Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double d = R * c;

            return d * 1000; // meters
        }
    }
}
