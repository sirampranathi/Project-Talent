using BoDi;
using MVPStudio.Framework.Helps;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace MVPStudio.Framework.Base
{
    public class WebDriverSetup
    {
        private readonly IObjectContainer _objectContainer;
        public IWebDriver Driver { get; set; }

        public WebDriverSetup(IObjectContainer objectContainer)
        {
            _objectContainer = objectContainer;
            // to stop the exception "The HTTP request to the remote WebDriver server for URL http://localhost:52847/session/... timed out after 60 seconds.'

            //Driver = new FirefoxDriver();
            var startupPath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, "MVPStudio.Framework");
            Driver = new ChromeDriver(startupPath);
            _objectContainer.RegisterInstanceAs(Driver);

        }
    }
}