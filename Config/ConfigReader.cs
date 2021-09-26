using MVPStudio.Framework.Helps;
using Newtonsoft.Json;
using System.IO;

namespace MVPStudio.Framework.Config
{
    public class ConfigReader
    {
        public static void InitializeFrameworkSettings()
        {
            {
                var appRoot = PathHelper.ToApplicationPath("Config\\settings.json");
                using (StreamReader stream = new StreamReader(appRoot))
                {
                    var json = stream.ReadToEnd();
                    JsonConvert.DeserializeObject<Settings>(json);
                }
            }
        }
    }
}