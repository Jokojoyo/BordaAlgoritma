using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace BordaAlgorithm.Utilities
{
    public class UCRUD
    {
        private string connection_string;

        public UCRUD(string pConnStr)
        {
            //this.connection_string = ConfigurationManager.ConnectionStrings[pConnStr].ConnectionString;
            this.connection_string = new ConnectionString().Items[pConnStr];
        }

        public SqlConnection GetConnection()
        {
            SqlConnection conn = new SqlConnection(connection_string);
            return conn;
        }

        public SqlConnection GetSqlConnection()
        {
            SqlConnection conn = new SqlConnection(connection_string);
            conn.Open();
            return conn;
        }

        public String SaveDataTable(DataTable data, string table, string whereClause)
        {
            try
            {
                using (SqlConnection conn = GetSqlConnection())
                {
                    using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM " + table + " " + whereClause, conn))
                    {
                        using (SqlCommandBuilder cb = new SqlCommandBuilder(da))
                        {
                            da.UpdateCommand = cb.GetUpdateCommand();
                            da.Update(data);
                        }
                    }
                }
                return "OK";
            }
            catch (Exception e)
            {
                return "ERROR : " + e.ToString();
            }
        }

        public String GetDataTable(out DataTable datatable, string query, List<SqlParameter> sqlParams)
        {
            DataSet ds = new DataSet();
            String ret = GetDataSet(out ds, query, sqlParams);
            datatable = ds.Tables.Count <= 0 ? new DataTable() : ds.Tables[0];
            return ret;
        }

        public String GetDataSet(out DataSet dataset, string query, List<SqlParameter> sqlParams)
        {
            dataset = new DataSet();
            try
            {
                using (SqlConnection conn = GetSqlConnection())
                {
                    using (SqlDataAdapter da = new SqlDataAdapter())
                    {
                        using (SqlCommand comm = new SqlCommand(query, conn))
                        {
                            try
                            {
                                if (sqlParams != null)
                                {
                                    foreach (SqlParameter param in sqlParams)
                                    {
                                        comm.Parameters.Add(param);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                return ex.Message;
                            }
                            da.SelectCommand = comm;
                            da.Fill(dataset);
                            conn.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "OK";
        }

        public String GetDataSet(out DataSet dataset, string sql_query)
        {
            dataset = new DataSet();
            try
            {
                using (SqlConnection conn = GetSqlConnection())
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(sql_query, conn))
                    {
                        da.Fill(dataset);
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "OK";
        }
    }
}