using AventStack.ExtentReports.Reporter;
using MVPStudio.Framework.Config;
using MVPStudio.Framework.Helps;

namespace MVPStudio.Framework.Base
{
    public class ReportContext
    {
        public static AventStack.ExtentReports.ExtentReports ExtendReport { get; set; }

        static ReportContext()
        {
            ExtendReport = new AventStack.ExtentReports.ExtentReports();
            var path = PathHelper.ToApplicationPath(Settings.ExtendReportPath);
            var htmlReporter = new ExtentHtmlReporter(path);
            htmlReporter.Config.Theme = AventStack.ExtentReports.Reporter.Configuration.Theme.Dark;
            ReportContext.ExtendReport.AttachReporter(htmlReporter);
        }
    }
}