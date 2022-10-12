using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace BordaAlgorithm.Utilities
{
    public class WTable
    {
        public UCRUD crud;
        public DataSet DS;
        public DataTable DT;
        public DataView DV;

        public string connSetting = "";
        public string query = "";
        public string tableName = "";
        public List<SqlParameter> sqlParams = new List<SqlParameter>();

        public WTable(string pConnSetting = "")
        {
            if (!string.IsNullOrEmpty(pConnSetting))
                connSetting = pConnSetting;
            else
                connSetting = "DefaultConnection";
        }

        public string Open(string pTable, string pQuery, List<SqlParameter> pSqlParams)
        {
            try
            {
                ConnectionString constr = new ConnectionString();
                using (SqlConnection conn = new SqlConnection(constr.Items[connSetting]))
                {
                    using (SqlCommand CMD = new SqlCommand(pQuery, conn))
                    {
                        if (pSqlParams != null)
                        {
                            foreach (SqlParameter param in pSqlParams)
                            {
                                // CMD.Parameters.Add(param);
                                CMD.Parameters.Add(param);
                            }
                        }
                        using (SqlDataAdapter DA = new SqlDataAdapter(CMD))
                        {
                            using (SqlCommandBuilder objCommandBuilder = new SqlCommandBuilder(DA))
                            {
                                DS = new DataSet();
                                DA.Fill(DS, pTable);
                            }
                        }
                    }
                }

                DT = new DataTable();
                DT = DS.Tables[pTable];
                DV = new DataView(DT);

                tableName = pTable;
                query = pQuery;
                sqlParams = pSqlParams;
                return "OK";
            }
            catch (Exception ex)
            {
                DS = new DataSet();
                DT = new DataTable();
                return ex.Message;
            }
        }

        public string Save()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(new ConnectionString().Items[connSetting]))
                {
                    if (conn.State != ConnectionState.Open)
                        conn.Open();
                    using (SqlCommand command = new SqlCommand("SELECT * FROM " + tableName, conn))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            using (SqlCommandBuilder builder = new SqlCommandBuilder(adapter))
                            {
                                builder.GetUpdateCommand();
                                adapter.Update(DT);
                                DT.AcceptChanges();
                            }
                        }
                    }
                }

                return "OK";
            }
            catch (Exception ex)
            {
                return query + "<br>" + ex.Message;
            }
        }

        public void Close()
        {
            DT.Clear();
            DS.Clear();
        }

        public void ReOpen()
        {

        }
    }

    public class ConnectionString
    {
        public Dictionary<string, string> Items;

        public ConnectionString()
        {
            // Baca configurasi dari web.config untuk menentukan posisi program di server DEV, QA atau PRD
            string pServer = ConfigurationManager.ConnectionStrings["Server"].ToString();
            this.Items = new Dictionary<string, string>();
            if ((string.IsNullOrEmpty(pServer)) || (pServer == "PRD"))
                this.Items.Add("DefaultConnection", "data source=IDBNG6219034661;initial catalog=DBBordaAlgorithm;persist security info=True;user id=sa;password=server!2345;Max Pool Size=200;");
            else if ((pServer == "DEV"))
                this.Items.Add("DefaultConnection", "data source=IDBNG6219034661;initial catalog=DBBordaAlgorithm;persist security info=True;user id=sa;password=server!2345;Max Pool Size=200;");
        }
    }
    public class Interpreter
    {
    }
}