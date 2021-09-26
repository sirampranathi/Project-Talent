using MVPStudio.Framework.Config;
using OpenQA.Selenium;
using System;
using System.IO;

namespace MVPStudio.Framework.Helps
{
    public class ScreenshotHelper
    {
        public static string TryToTakeScreenshot(IWebDriver driver)
        {
            var screenshotTaker = driver as ITakesScreenshot;
            try
            {
                var screenshot = screenshotTaker.GetScreenshot();
                var screenshotFilePath = CreateScreenshotFilePath();
                screenshot.SaveAsFile(screenshotFilePath, ScreenshotImageFormat.Png);
                return screenshotFilePath;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static string CreateScreenshotFilePath()
        {
            var screenshotFolderPath = PathHelper.ToApplicationPath(Settings.ScreenshotPath);
            var screenshotFileName = "screenshot" + DateTime.Now.ToString("_MM_dd_yyyy_HH-mm") + ".png";
            return Path.Combine(screenshotFolderPath, screenshotFileName);
        }
    }
}