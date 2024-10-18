namespace SAPB1SLayerWebAPI.Utils
{
    public class Logger
    {
        public static string LIVE_PATH = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\PORTAL_LOGS\\";

        public static void CreateLog(bool isError, string title, string message, string data)
        {
            Directory.CreateDirectory(string.Format("{0}\\ERRORS", LIVE_PATH));
            Directory.CreateDirectory(string.Format("{0}\\INFO", LIVE_PATH));

            string dateTimeToday = DateTime.Now.ToString("yyyyMMdd");
            string file = isError ? "ERRORS\\" : "INFO\\";
            StreamWriter sw = new(LIVE_PATH + file + dateTimeToday + ".txt", true);
            sw.WriteLine(DateTime.Now.ToString("HH:mm:ss ") + title + ": ");
            sw.WriteLine(message);
            sw.WriteLine(data);
            sw.WriteLine("============================================================");
            sw.Close();
        }
    }
}
