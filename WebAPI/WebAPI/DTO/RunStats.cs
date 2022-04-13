using WebAPI.Models;

namespace WebAPI.DTO
{
    public class RunStats
    {
        public RunStats()
        {

        }

        /// <summary>
        /// Takes a Run object.
        /// Returns a RunStats DTO that contains information derived from original objects data.
        /// </summary>
        /// <param name="run"></param>
        public RunStats(Run run)
        {
            //Data recycled from original Run object
            dateTime = run.dateTime;
            points = run.points;

            //Calculate total distance
            distance = 0;
            for (int i = 0; i < run.points.Count() - 1; i++)
            {
                distance += latLongDistanceiInMeters(run.points[i].latitude, run.points[i].longitude, run.points[i + 1].latitude, run.points[i + 1].longitude);
            }

            //Calculate total duration
            //Use first point to last point for accuracy
            //If only one point in run, use run.dateTime to last point, to avoid dividing by zero later (this makes duration result for Runs with only one point slightly less accurate)
            duration = run.points.Last().dateTime.Subtract(run.points.First().dateTime);
            if (duration == TimeSpan.Zero)
            {
                duration = run.points.Last().dateTime.Subtract(run.dateTime);
            }

            //Calculate average speed
            avgSpeedInMetersPerSecond = distance / duration.TotalSeconds;

            //Calculate average speed pr minute (for chart)
            avgSpeedPerMinuteInMetersPerSecond = new List<double>();
            avgSpeedPerMinuteInMetersPerSecond.Add(0);
            int minute = 1;
            //Number of parts to split remainders between minutes into. With the frequency of points being no more than a few seconds, 1000 should be plenty accurate
            int restPrecisionFactor = 1000;
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
                        if (run.points[i1].dateTime + (i2 * restTime) < run.dateTime.AddMinutes(minute)) //This minute
                        {
                            avgSpeedPerMinuteInMetersPerSecond[minute - 1] += restDistance;
                        }
                        else //Next minute
                        {
                            avgSpeedPerMinuteInMetersPerSecond[minute] += restDistance;
                        }
                    }

                    minute++; //Increment minute
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
                    //Last minute might not be a whole minute, so divide by the number of seconds remaining instead.
                    //castings to double to force decimal results.
                    avgSpeedPerMinuteInMetersPerSecond[i] = avgSpeedPerMinuteInMetersPerSecond[i] / ((double)duration.Seconds + ((double)duration.Milliseconds / (double)1000));
                }
            }
        }

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
