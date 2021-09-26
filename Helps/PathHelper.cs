using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MVPStudio.Framework.Helps
{
    public static class PathHelper
    {
        public static string ToApplicationPath(string fileName)
        {
            var exePath = Path.GetDirectoryName(
                                Assembly.GetExecutingAssembly().CodeBase);
            Regex appPathMatcher = new Regex(@"(?<!fil)[A-Za-z]:\\+[\S\s]*?(?=\\+bin)");
            var appRoot = appPathMatcher.Match(exePath).Value;
            return Path.Combine(appRoot, fileName);
        }
    }
}