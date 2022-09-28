using System;
using System.Globalization;
using MCHMIS.Mobile.Database;
using Xamarin.Forms;

namespace MCHMIS.Mobile.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        public static string[] WinStreakColors = new string[] { "#CEF6CE", "#A9F5A9", "#81F781", "#58FA58", "#2EFE2E", "#00FF00", "#01DF01" };
        public static string[] LooseStreakColors = new string[] { "#F5A9A9", "#F78181", "#FA5858", "#FE2E2E", "#FF0000", "#DF0101", "8A0808" };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Color.Transparent;
            var code = "PENDING";
            var val = value.ToString();
            if (!string.IsNullOrEmpty(val))
                code = ((SystemCodeDetail)App.Database.GetTableRow("SystemCodeDetail", "Id", val)).Code;

            if (code == "REGEXIT")
                return Color.FromHex((LooseStreakColors[5]));
            if (code == "REGCONFIRM")
                return Color.FromHex((LooseStreakColors[1]));
            else if (code == "REGAPV")
                return Color.FromHex((WinStreakColors[1]));
            else if (code == "REGCORRECT")
                return Color.FromHex((WinStreakColors[5]));
            else if (code == "REGCORRECT")
                return Color.FromHex((WinStreakColors[6]));

            return Color.FromHex((LooseStreakColors[1]));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}