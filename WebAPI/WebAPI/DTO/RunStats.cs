using WebAPI.Models;

namespace WebAPI.DTO
{
    public class RunStats
    {
        public RunStats()
        {

        }

        public RunStats(Run run, double distance, TimeSpan duration, float avgSpeedInKmt, List<float> avgSpeedInKmtPerMinute)
        {
            dateTime = run.dateTime;
            points = run.points;

            this.distance = distance;
            this.duration = duration;
            this.avgSpeedInKmt = avgSpeedInKmt;
            this.avgSpeedInKmtPerMinute = avgSpeedInKmtPerMinute;
        }

        public DateTime dateTime { get; set; }

        public List<Point> points { get; set; }

        public double distance { get; set; }

        public TimeSpan duration { get; set; }

        public float avgSpeedInKmt { get; set; }

        public List<float> avgSpeedInKmtPerMinute { get; set; }
    }
}
