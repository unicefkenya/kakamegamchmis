using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MCHAPP
{
    static class Program
    {
        public static string FacilityName { get; set; }
        public static int FacilityId { get; set; }
        public static string UserId { get; set; }
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MCHMDI());
        }
    }
}
