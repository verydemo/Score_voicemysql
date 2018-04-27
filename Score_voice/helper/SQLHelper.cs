using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Data.SqlClient;
using System.Configuration;


namespace Score_voice
{
    /// <summary>
    ///  通用数据访问类
    /// </summary>
    public class SQLHelper
    {
        //public static readonly string connString =ConfigurationManager.ConnectionStrings["connString"].ToString();
        //public static System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        //public static string connString = config.AppSettings.Settings["Connstring"].Value;
        public static string connString = "server=172.29.83.38,1899;database=HYT_Score;uid=SA;pwd=zjl12345^";
        /// <summary>
        /// 建立数据库连接.（测试连接）
        /// </summary>
        /// <returns>返回MySqlConnection对象</returns>
        public static bool TestSqlConn()
        {
            try
            {
                SqlConnection myCon = new SqlConnection(connString);
                myCon.Open();
                myCon.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }


        }

        /// <summary>
        /// 执行增、删、改（insert/update/delete）
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static int Update(string sql)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(sql, conn);
            try
            {
                conn.Open();
                int result = cmd.ExecuteNonQuery();
                
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }
        }
        /// <summary>
        /// 执行单一结果查询（select） 返回Id
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static int GetSingleResult(string sql)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(sql, conn);
            try
            {
                conn.Open();
                //object result = cmd.ExecuteScalar();
                int Id = Convert.ToInt32(cmd.ExecuteScalar().ToString());
                return Id;
            }
            catch (Exception)
            {
                //throw ex;
                return 0;
            }
            finally
            {
                conn.Close();
            }
        }
        /// <summary>
        /// 执行多结果查询（select）
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static SqlDataReader GetReader(string sql)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(sql, conn);
            try
            {
                conn.Open();
                SqlDataReader objReader =
                    cmd.ExecuteReader(CommandBehavior.CloseConnection);
                return objReader;
            }
            catch (Exception ex)
            {
                conn.Close();
                throw ex;
            }
        }
        /// <summary>
        /// 执行返回数据集的查询
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static DataSet GetDataSet(string sql)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataAdapter da = new SqlDataAdapter(cmd); //创建数据适配器对象
            DataSet ds = new DataSet();//创建一个内存数据集
            try
            {
                conn.Open();
                da.Fill(ds);  //使用数据适配器填充数据集
                return ds;  //返回数据集
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// SqlBulkCopy批量插入数据
        /// </summary>
        /// <param name="connectionStr">链接字符串</param>
        /// <param name="dataTableName">表名</param>
        /// <param name="sourceDataTable">数据源</param>
        /// <param name="batchSize">一次事务插入的行数</param>
        public static void SqlBulkCopyByDataTable(string dataTableName, DataTable sourceDataTable, int batchSize = 10000)
        {
            string connectionStr = connString;
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(connectionStr, SqlBulkCopyOptions.UseInternalTransaction))
                {
                    try
                    {
                        sqlBulkCopy.DestinationTableName = dataTableName;
                        sqlBulkCopy.BatchSize = batchSize;
                        for (int i = 0; i < sourceDataTable.Columns.Count; i++)
                        {
                            sqlBulkCopy.ColumnMappings.Add(sourceDataTable.Columns[i].ColumnName, sourceDataTable.Columns[i].ColumnName);
                        }
                        sqlBulkCopy.WriteToServer(sourceDataTable);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }

        /// <param name="table">准备更新的DataTable新数据</param>
        /// <param name="TableName">对应要更新的数据库表名</param>
        /// <param name="primaryKeyName">对应要更新的数据库表的主键名</param>
        /// <param name="columnsName">对应要更新的列的列名集合</param>
        /// <param name="limitColumns">需要在ＳＱＬ的ＷＨＥＲＥ条件中限定的条件字符串，可为空。</param>
        /// <param name="onceUpdateNumber">每次往返处理的行数</param>
        /// <returns>返回更新的行数</returns>
        //public static int bigUpdate(DataTable table, string TableName, string primaryKeyName, List<columnsstr> columnsstr, string limitWhere, int onceUpdateNumber)
        //{
        //    string connectionStr = connString;
        //    if (string.IsNullOrEmpty(TableName)) return 0;
        //    if (string.IsNullOrEmpty(primaryKeyName)) return 0;
        //    if (columnsstr == null || columnsstr.Count <= 0) return 0;
        //    DataSet ds = new DataSet();
        //    table.TableName = "HYT_Score";
        //    ds.Tables.Add(table);
        //    int result = 0;
        //    using (SqlConnection sqlconn = new SqlConnection(connectionStr))
        //    {
        //        sqlconn.Open();

        //        //使用加强读写锁事务   
        //        SqlTransaction tran = sqlconn.BeginTransaction(IsolationLevel.ReadCommitted);
        //        try
        //        {
        //            foreach (DataRow dr in ds.Tables[0].Rows)
        //            {
        //                //所有行设为修改状态   
        //                dr.SetModified();
        //            }
        //            //为Adapter定位目标表   
        //            SqlCommand cmd = new SqlCommand(string.Format("select * from {0} where {1}", TableName, limitWhere), sqlconn, tran);
        //            SqlDataAdapter da = new SqlDataAdapter(cmd);
        //            da.AcceptChangesDuringFill = false;
        //            SqlCommandBuilder sqlCmdBuilder = new SqlCommandBuilder(da);
        //            da.AcceptChangesDuringUpdate = false;
        //            string columnsUpdateSql = "";
        //            foreach (var c in columnsstr)
        //            { 
        //                columnsUpdateSql+=string.Format("{0}=@{1},",c.columnsName,c.columnsName);
        //            }
        //            if (!string.IsNullOrEmpty(columnsUpdateSql))
        //            {
        //                //此处去掉拼接处最后一个","
        //                columnsUpdateSql = columnsUpdateSql.Remove(columnsUpdateSql.Length - 1);
        //            }
        //            //此处生成where条件语句
        //            string limitSql = ("[" + primaryKeyName + "]" + "=@" + primaryKeyName);
        //            string text = string.Format(" UPDATE {0} SET {1} WHERE {2} ", TableName, columnsUpdateSql, limitSql);
        //            SqlCommand updateCmd = new SqlCommand(text);
        //            //不修改源DataTable   
        //            da.UpdateCommand = updateCmd;
        //            foreach (var c in columnsstr)
        //            {
        //                da.UpdateCommand.Parameters.Add("@" + c.columnsName, c.SqlDbType, c.size, c.columnsName);
        //            }
        //            da.UpdateCommand.Parameters.Add("@" + primaryKeyName, SqlDbType.Int, 5000, primaryKeyName);
        //            updateCmd.UpdatedRowSource = UpdateRowSource.None;
        //            //每次往返处理的行数
        //            da.UpdateBatchSize = onceUpdateNumber;
        //            result = da.Update(ds, TableName);
        //            ds.AcceptChanges();
        //            tran.Commit();

        //        }
        //        catch (Exception)
        //        {
        //            //throw ex;
        //            tran.Rollback();
        //        }
        //        finally
        //        {
        //            sqlconn.Dispose();
        //            sqlconn.Close();
        //        }
        //    }
        //    return result;
        //}

        /// <summary>
        /// 返一DATATABLE数据表集
        /// </summary>
        /// <param name="sqlText">传入的SQL语句</param>
        /// <returns></returns>
        public static DataTable GetDbTable(string sql)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataAdapter da = new SqlDataAdapter(cmd); //创建数据适配器对象
            DataSet ds = new DataSet();//创建一个内存数据集
            try
            {
                conn.Open();
                da.Fill(ds);  //使用数据适配器填充数据集
                return ds.Tables[0];  //返回数据集
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }
           
        }

        /// <summary>
        /// 获取服务器的时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetServerTime()
        {
            return Convert.ToDateTime(GetSingleResult("select getdate()"));
        }

        public static void BatchUpdate(DataTable dataTable, Int32 batchSize)
        {
            // Assumes GetConnectionString() returns a valid connection string.
            //string connectionString = GetConnectionString();
            string connectionString = connString;
            // Connect to the AdventureWorks database.
            using (SqlConnection connection = new
              SqlConnection(connectionString))
            {

                // Create a SqlDataAdapter.
                SqlDataAdapter adapter = new SqlDataAdapter();

                // Set the UPDATE command and parameters.
                adapter.UpdateCommand = new SqlCommand(
                    "UPDATE Production.ProductCategory SET "
                    + "Name=@Name WHERE ProductCategoryID=@ProdCatID;",
                    connection);
                adapter.UpdateCommand.Parameters.Add("@Name",
                   SqlDbType.NVarChar, 50, "Name");
                adapter.UpdateCommand.Parameters.Add("@ProdCatID",
                   SqlDbType.Int, 4, "ProductCategoryID");
                adapter.UpdateCommand.UpdatedRowSource = UpdateRowSource.None;

                // Set the INSERT command and parameter.
                adapter.InsertCommand = new SqlCommand(
                    "INSERT INTO Production.ProductCategory (Name) VALUES (@Name);",
                    connection);
                adapter.InsertCommand.Parameters.Add("@Name",
                  SqlDbType.NVarChar, 50, "Name");
                adapter.InsertCommand.UpdatedRowSource = UpdateRowSource.None;

                // Set the DELETE command and parameter.
                adapter.DeleteCommand = new SqlCommand(
                    "DELETE FROM Production.ProductCategory "
                    + "WHERE ProductCategoryID=@ProdCatID;", connection);
                adapter.DeleteCommand.Parameters.Add("@ProdCatID",
                  SqlDbType.Int, 4, "ProductCategoryID");
                adapter.DeleteCommand.UpdatedRowSource = UpdateRowSource.None;

                // Set the batch size.
                adapter.UpdateBatchSize = batchSize;

                // Execute the update.
                adapter.Update(dataTable);
            }
        }




        public static List<T> GetDataList<T>(string sql)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataAdapter da = new SqlDataAdapter(cmd); //创建数据适配器对象
            DataSet ds = new DataSet();//创建一个内存数据集
            try
            {
                conn.Open();
                da.Fill(ds);  //使用数据适配器填充数据集
                List<T> DataList = IListDataSet.DataSetToIList<T>(ds).ToList();
                return DataList;  //返回数据列表
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }
        }
    }
}

