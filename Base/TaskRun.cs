using KGQT.Business;
using KGQT.Business.Base;
using KGQT.Models;
using KGQT.Models.temp;
using System.IO.Packaging;

namespace KGQT.Base
{
    public class TaskRun
    {
        private static TaskRun _instance;
        public static bool isRuning = false;
        private List<Timer> timers = new List<Timer>();

        private TaskRun()
        {
        }

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
            var config = BusinessBase.Get<tbl_Schedules>().ToList();
            #region Baidu Task
            var baidu = config.FirstOrDefault(x=>x.TaskID == 1003);
            int bHours = 16;
            int bMinute = 0;
            int bSecond = 0;

            if(baidu != null)
            {
                bHours = baidu.Hours.Value;
                bMinute = baidu.Minute.Value;
                bSecond = baidu.Seccon.Value;
            }

            Instance.ScheduleTask(bHours, bMinute, bSecond,
               () =>
               {
                   //here write the code that you want to schedule
                   PackagesBusiness.DailyTaskAsync();
               });
            #endregion

            #region OA follower
            var follower = config.FirstOrDefault(x => x.TaskID == 1002);
            int fHours = 23;
            int fMinute = 59;
            int fSecond = 0;

            if (follower != null)
            {
                fHours = follower.Hours.Value;
                fMinute = follower.Minute.Value;
                fSecond = follower.Seccon.Value;
            }

            Instance.ScheduleTask(fHours, fMinute, fSecond,
             () =>
             {
                 //here write the code that you want to schedule
                 ZaloCommon.GetListFollower();
             });
            #endregion

            #region Send Request
            //var oa = config.FirstOrDefault(x => x.TaskID == 1001);
            //int oHours = 9;
            //int oMinute = 0;
            //int oSecond = 0;

            //if (oa != null)
            //{
            //    oHours = oa.Hours.Value;
            //    oMinute = oa.Minute.Value;
            //    oSecond = oa.Seccon.Value;
            //}

            //Instance.ScheduleTask(oHours, oMinute, oSecond, () =>
            //{
            //    ZaloCommon.SendRequestAuto();
            //});
            #endregion
        }
    }
}
