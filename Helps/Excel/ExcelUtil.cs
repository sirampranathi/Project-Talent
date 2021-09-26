using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace MVPStudio.Framework.Helps.Excel
{
    /*
     *       Usage for this class:
     *       1. Set the Data source(which excel file you want to use) in the class constructor -> ExcelUtil.SetDataSource("credential.xlsx");
     *       2. Set the Sheet you want to get data from -> ExcelUtil.DataSet.SelectSheet("ANY")
     *       3. Get the row data by key(the key column in the excel sheet)
     *       4. var rowData = ExcelUtil.DataSet.SelectSheet("ANY").GetRowByKey("ANY");
     *       5. You can access the rowData by providing colnum name -> rowData.GetValue("ANY")
     */
    public static class ExcelUtil
    {

        private static readonly List<ExcelModel> _ExcelMemoryData = new List<ExcelModel>();
        public static ExcelData DataSet { get; set; } = new ExcelData();
        internal static void InitExcelData()
        {
            PopulateDataIntoMemory("credential.xlsx");
            PopulateDataIntoMemory("profile.xlsx");
            PopulateDataIntoMemory("articleScheduler.xlsx");
            PopulateDataIntoMemory("article.xlsx");
            PopulateDataIntoMemory("employer.xlsx");
            PopulateDataIntoMemory("Job.xlsx");
        }

        private static void PopulateDataIntoMemory(string fileName)
        {
            try
            {
                var dataFromExcel = ExcelToDataTable(PathHelper.ToApplicationPath($"TestData\\{fileName}"));
                for (int i = 0; i < dataFromExcel.Count; i++)
                {
                    for (int row = 1; row <= dataFromExcel[i].Rows.Count; row++)
                    {
                        var excelModel = new ExcelModel()
                        {
                            Key = dataFromExcel[i].Rows[row - 1]["key"].ToString(),
                            DataSet = new List<DataSetModel>(),
                            Sheet = dataFromExcel[i].TableName,
                            ExcelFileName = fileName
                        };

                        for (int col = 0; col < dataFromExcel[i].Columns.Count; col++)
                        {
                            var dataSet = new DataSetModel
                            {
                                ColumnName = dataFromExcel[i].Columns[col].ToString(),
                                ColumnValue = dataFromExcel[i].Rows[row - 1][col].ToString()

                            };
                            excelModel.DataSet.Add(dataSet);
                        }
                        _ExcelMemoryData.Add(excelModel);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

        }
        private static DataTableCollection ExcelToDataTable(string fileName)
        {
            try
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                using (var stream = File.Open(fileName, FileMode.Open, FileAccess.Read))
                {
                    IExcelDataReader reader = ExcelReaderFactory.CreateOpenXmlReader(stream);

                    var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true
                        }
                    });

                    return result.Tables;

                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Set the Excel file(with extension) which you want to access
        /// </summary>
        /// <param name="excelFile">Excel file name with extension</param>
        /// <returns>Return a ExcelUtil class </returns>
        public static void SetDataSource(string excelFile)
        {
            DataSet.Data = _ExcelMemoryData.Where(x => x.ExcelFileName == excelFile).ToList();
        }
    }
}
