using System.Collections.Generic;

namespace MVPStudio.Framework.Helps.Excel
{
    public class ExcelModel
    {
        public List<DataSetModel> DataSet { get; set; }
        public string Key { get; set; }
        public string Sheet { get; set; }
        public string ExcelFileName { get; set; }
    }

    public class DataSetModel
    {
        public string ColumnName { get; set; }
        public string ColumnValue { get; set; }
    }

}