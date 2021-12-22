using System.Configuration;
using System.Threading.Tasks;

namespace AnnouncerBot
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }
        static async Task MainAsync()
        {
            string secretToken = ConfigurationManager.AppSettings.Get("BotToken");
            var bot = new Models.AnnouncerBot(secretToken);
            await bot.Connect();
        }

    }
}
