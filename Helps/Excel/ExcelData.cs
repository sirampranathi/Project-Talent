using System;
using System.Collections.Generic;
using System.Linq;

namespace MVPStudio.Framework.Helps.Excel
{
    /*
     *       Usage for this class:
     *       1. Set the Data source(which excel file you want to use) -> ExcelUtil.SetDataSource("credential.xlsx");
     *       2. Set the Sheet you want to get data from -> ExcelUtil.DataSet.SelectSheet("ANY")
     *       3. Get the row data by key(the key column in the excel sheet)
     *       4. var rowData = ExcelUtil.DataSet.SelectSheet("ANY").GetRowByKey("ANY");
     *       5. You can access the rowData by providing colnum name -> rowData.GetValue("ANY")
     */
    public class ExcelData
    {
        internal List<ExcelModel> Data { get; set; }
        internal List<DataSetModel> DataSet { get; set; }

        /// <summary>
        /// Get all the values for the entire row which has the same column name
        /// </summary>
        /// <param name="colName"></param>
        /// <returns></returns>
        public IList<string> GetValues(string colName)
        {
            try
            {
                var values = Data
                        .Select(x => x.DataSet.Where(y => y.ColumnName == colName).Select(y => y.ColumnValue).SingleOrDefault()).ToList();

                return values;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Get single value of the colnum by the name
        /// </summary>
        /// <param name="colName"></param>
        /// <returns></returns>
        public string GetValue(string colName)
        {
            return DataSet.Where(x => x.ColumnName == colName).Select(y => y.ColumnValue).SingleOrDefault();
        }


        /// <summary>
        /// Get the list of data which match the key with the key in Excel
        /// </summary>
        /// <param name="key">Any key that existed in the provided excel</param>
        /// <returns>Return a ExcelUtil class that match with the key</returns>
        public ExcelData GetRowByKey(string key)
        {
            var result = Data.Where(x => x.Key == key).SelectMany(x => x.DataSet.ToList()).ToList();
            if (result.Count == 0 || result == null)
            {
                throw new Exception();
            }

            DataSet = result;
            return this;
        }

        /// <summary>
        /// You should call this method before using any function in this class
        /// </summary>
        /// <param name="key">Any key that existed in the provided excel</param>
        /// <returns>Return ExcelData and ready for the method chaining</returns>
        public ExcelData SelectSheet(string sheet)
        {
            Data = Data.Where(x => x.Sheet == sheet).ToList();
            return this;
        }
        /// <summary>
        /// Get a list of data by column name
        /// </summary>
        /// <param name="column">The column name in the excel</param>
        /// <returns>Return a list of value where equal to column</returns>
        public DataSetModel GetRowByColumn(string column)
        {
            return Data.SelectMany(x => x.DataSet.Where(y => y.ColumnName == column)).SingleOrDefault();
        }

        /// <summary>
        /// Get the entire data set that is from the excel
        /// </summary>
        /// <returns>Return list of dataset in the row </returns>
        public List<ExcelModel> ToList()
        {
            return Data.ToList();
        }
    }
}
