using KGQT.Business;
using System.IO.Packaging;

namespace KGQT.Base
{
    public class TaskRun
    {
        private static TaskRun _instance;
        public static bool isRuning = false;
        private List<Timer> timers = new List<Timer>();

        private TaskRun() { }

        public static TaskRun Instance => _instance ?? (_instance = new TaskRun());

        public void ScheduleTask(int hour, int min, double intervalInHour, Action task)
        {
            DateTime now = DateTime.Now;
            DateTime firstRun = new DateTime(now.Year, now.Month, now.Day, hour, min, 0, 0);
            if (now > firstRun)
            {
                firstRun = firstRun.AddDays(1);
            }

            TimeSpan timeToGo = firstRun - now;
            if (timeToGo <= TimeSpan.Zero)
            {
                timeToGo = TimeSpan.Zero;
            }

            var timer = new Timer(x =>
            {
                task.Invoke();
            }, null, timeToGo, TimeSpan.FromHours(intervalInHour));

            timers.Add(timer);
        }

        public static void Run()
        {
            Instance.ScheduleTask(16, 0, 0,
               () =>
               {
                   //here write the code that you want to schedule
                   PackagesBusiness.DailyTask();
               });
            Instance.ScheduleTask(23, 59, 0,
             () =>
             {
                 //here write the code that you want to schedule
                 ZaloCommon.GetListFollower();
             });

            Instance.ScheduleTask(9, 0, 0, () =>
            {
                ZaloCommon.SendRequestAuto();
            });
        }
    }
}
