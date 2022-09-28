using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MCHMIS.Services
{
    public interface ILogService
    {
        void DBLog(string type, string exc, string category, string item, string UserId);

        void FileLog(string dataType, string data);

        void LogWrite(string type, string exc, string category, string item, string UserId);

        // string GetErrorListFromModelState(ModelStateDictionary modelState);
    }

    public class LogService : ILogService
    {
        private string path = "";
        private readonly IHostingEnvironment _hostingEnvironment;

        public LogService(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            string webRootPath = _hostingEnvironment.WebRootPath;
            path = webRootPath + "/Logs/";
        }
     
        public void DBLog(string type, string exc, string category, string item, string UserId)
        {
            
        }
        //public void FileLog(string dataType, string data)
        //{
        //    string t;
        //    int seconds;
        //    string todaydate, hour;
        //    var dt = DateTime.UtcNow.AddHours(3);
        //    seconds = dt.Second;
        //    todaydate = dt.Date.ToString("yyyy-MM-dd");
        //    var minute = dt.Date.ToString("mm");
        //    hour = dt.TimeOfDay.Hours.ToString();
        //    if (!Equals(seconds, dt.Second))
        //    {
        //        seconds = dt.Second;
        //    }

        //    t = dt.ToString("T");
        //    var fs = new FileStream(  $"{this.path}{dataType}-{todaydate}.txt",  FileMode.OpenOrCreate,  FileAccess.Write);
        //    using (var mStreamWriter = new StreamWriter(fs))
        //    {
        //        mStreamWriter.BaseStream.Seek(0, SeekOrigin.End);
        //        mStreamWriter.WriteLine(data);
        //        mStreamWriter.Flush();
        //        mStreamWriter.Close();
        //    }
        //}
        public void FileLog(string dataType, string data)
        {
            string t;
            int seconds;
            string todaydate, hour;
            var dt = DateTime.UtcNow.AddHours(3);
            seconds = dt.Second;
            todaydate = dt.Date.ToString("yyyy-MM-dd");
            var minute = dt.Date.ToString("mm");
            hour = dt.TimeOfDay.Hours.ToString();
            if (!Equals(seconds, dt.Second))
            {
                seconds = dt.Second;
            }
            t = dt.ToString("T");
            var fs = new FileStream(     $"{this.path}{dataType}-{todaydate}-{hour}-00.txt",  FileMode.OpenOrCreate, FileAccess.Write);
            using (var mStreamWriter = new StreamWriter(fs))
            {
                mStreamWriter.BaseStream.Seek(0, SeekOrigin.End);
                mStreamWriter.WriteLine("||            Date: {0}   Time : {1}", todaydate, t);
              

                // mStreamWriter.WriteLine("|||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");
                mStreamWriter.WriteLine(
                    " ****************************************************************************************************************************");
                mStreamWriter.WriteLine(" POSTED DATA   " + data);
                mStreamWriter.WriteLine(
                    " ****************************************************************************************************************************");

                mStreamWriter.Flush();
                mStreamWriter.Close();
            }
        }
        public void LogWrite(string type, string exc, string category, string item, string UserId)
        {
            string t;
            int seconds;
            string todaydate, hour;
            var dt = DateTime.UtcNow.AddHours(3);
            seconds = dt.Second;
            todaydate = dt.Date.ToString("yyyy-MM-dd");
            var minute = dt.Date.ToString("mm");
            hour = dt.TimeOfDay.Hours.ToString();
            if (!Equals(seconds, dt.Second))
            {
                seconds = dt.Second;
            }

            t = dt.ToString("T");
            var fs = new FileStream(
                $"{this.path}-{todaydate}-{hour}-00.txt",
                FileMode.OpenOrCreate,
                FileAccess.Write);
            using (var mStreamWriter = new StreamWriter(fs))
            {
                mStreamWriter.BaseStream.Seek(0, SeekOrigin.End);

                mStreamWriter.WriteLine("");
                mStreamWriter.WriteLine(exc);

                mStreamWriter.WriteLine("");
                mStreamWriter.WriteLine(exc);

                mStreamWriter.Flush();
                mStreamWriter.Close();
            }
        }
        public void MailLog(string type, string exc, string category, string item, string UserId)
        {
        }
    }
}