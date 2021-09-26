using MVPStudio.Framework.Config;
using MVPStudio.Framework.Helps.Excel;

namespace MVPStudio.Framework.Base
{
    public class FrameworkInitializeHook
    {
        public void InitializeSettings()
        {
            ConfigReader.InitializeFrameworkSettings();

            ExcelUtil.InitExcelData();

            _ = new ReportContext();
        }

    }
}
