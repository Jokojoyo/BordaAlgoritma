using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;

namespace BordaAlgorithm.Utilities
{
    public class ColumnClass
    {
        public string data { get; set; }
        public string name { get; set; }
        public Nullable<bool> orderable { get; set; }
        public Nullable<bool> visible { get; set; }
        public Nullable<bool> searchable { get; set; }
    }

    public class OrderClass
    {
        public int column { get; set; }
        public string dir { get; set; }
    }

    public class BaseProgram
    {
        private const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty;
        public static bool UserAuthenticated()
        {
            return (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
        }
        public static void CopyProperties(Type fromType, object from, Type toType, object to)
        {
            if (fromType == null)
                throw new ArgumentNullException("fromType", "The type that you are copying from cannot be null");

            if (from == null)
                throw new ArgumentNullException("from", "The object you are copying from cannot be null");

            if (toType == null)
                throw new ArgumentNullException("toType", "The type that you are copying to cannot be null");

            if (to == null)
                throw new ArgumentNullException("to", "The object you are copying to cannot be null");

            // Don't copy if they are the same object
            if (!ReferenceEquals(from, to))
            {
                // Get all of the public properties in the toType with getters and setters
                Dictionary<string, PropertyInfo> toProperties = new Dictionary<string, PropertyInfo>();
                PropertyInfo[] properties = toType.GetProperties(flags);
                foreach (PropertyInfo property in properties)
                {
                    toProperties.Add(property.Name, property);
                }

                // Now get all of the public properties in the fromType with getters and setters
                properties = fromType.GetProperties(flags);
                foreach (PropertyInfo fromProperty in properties)
                {
                    // If a property matches in name and type, copy across
                    if (toProperties.ContainsKey(fromProperty.Name))
                    {
                        PropertyInfo toProperty = toProperties[fromProperty.Name];
                        if (toProperty.PropertyType == fromProperty.PropertyType)
                        {
                            object value = fromProperty.GetValue(from, null);
                            toProperty.SetValue(to, value, null);
                            if (toProperty.GetValue(to) == null)
                            {
                                if (toProperty.Name.Contains(""))
                                {

                                }
                            }
                        }
                    }
                }
            }
        }

        public static void CopySomeProperties(Type fromType, object from, Type toType, object to, List<string> skipCopy)
        {
            if (!ReferenceEquals(from, to))
            {
                // Get all of the public properties in the toType with getters and setters
                Dictionary<string, PropertyInfo> toProperties = new Dictionary<string, PropertyInfo>();
                PropertyInfo[] properties = toType.GetProperties(flags);
                foreach (PropertyInfo property in properties)
                {
                    toProperties.Add(property.Name, property);
                }

                // Now get all of the public properties in the fromType with getters and setters
                properties = fromType.GetProperties(flags);
                foreach (PropertyInfo fromProperty in properties)
                {
                    // If a property matches in name and type, copy across
                    if (toProperties.ContainsKey(fromProperty.Name) && !skipCopy.Contains(fromProperty.Name.ToString()))
                    {
                        PropertyInfo toProperty = toProperties[fromProperty.Name];
                        if (toProperty.PropertyType == fromProperty.PropertyType)
                        {
                            object value = fromProperty.GetValue(from, null);
                            toProperty.SetValue(to, value, null);
                            if (toProperty.GetValue(to) == null)
                            {
                                if (toProperty.Name.Contains(""))
                                {

                                }
                            }
                        }
                    }
                }
            }
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static string GetDataTable(String pConnSetting,
                                          String query,
                                          Dictionary<string, object> parameters,
                                          out DataTable data,
                                          Boolean useExistingConnection = false,
                                          SqlConnection conn = null,
                                          SqlTransaction trans = null,
                                          bool increaseTimeOut = false)
        {
            data = new DataTable();
            string connString = String.IsNullOrEmpty(pConnSetting) ? new ConnectionString().Items["DefaultConnection"] : new ConnectionString().Items[pConnSetting];
            try
            {
                if (!useExistingConnection || conn == null)
                {
                    conn = new SqlConnection(connString);
                    conn.Open();
                }

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    if (useExistingConnection && trans != null) command.Transaction = trans;
                    if (increaseTimeOut || true)
                        command.CommandTimeout = 300;
                    foreach (KeyValuePair<string, object> parameter in parameters)
                    {
                        //command.Parameters.AddWithValue(parameter.Key, parameter.Value);
                        if (parameter.Value.GetType() == typeof(string))
                        {
                            command.Parameters.Add(new SqlParameter(parameter.Key, SqlDbType.VarChar, parameter.Value.ToString().Length, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Default, parameter.Value));
                        }
                        else
                            command.Parameters.AddWithValue(parameter.Key, parameter.Value);

                    }
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(data);
                    }
                }
            }
            catch (Exception exc)
            {
                return exc.Message;
            }
            finally
            {
                if (!useExistingConnection)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            return "OK";
        }

        public static double ConvertToDouble(string s)
        {
            char systemSeparator = System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator[0];
            double result = 0;
            try
            {
                if (s != null)
                    if (!s.Contains(","))
                        result = double.Parse(s, CultureInfo.InvariantCulture);
                    else
                    {
                        if (!s.Contains(systemSeparator))
                        {
                            result = Convert.ToDouble(s);
                        }
                        else
                        {
                            result = Convert.ToDouble(s.Replace(".", systemSeparator.ToString()).Replace(",", systemSeparator.ToString()));
                        }
                    }
            }
            catch (Exception e)
            {
                try
                {
                    result = Convert.ToDouble(s);
                }
                catch
                {
                    try
                    {
                        result = Convert.ToDouble(s.Replace(",", ";").Replace(".", ",").Replace(";", "."));
                    }
                    catch
                    {
                        //throw new Exception("Wrong string-to-double format");
                        result = 0;
                    }
                }
            }
            return result;
        }

        public static DataTable BordaAlgoritmaPoint()
        {
            //initalisasi data borda point
            DataTable dtPointBorda = new DataTable();
            dtPointBorda.Columns.Add("Category", typeof(string));
            dtPointBorda.Columns.Add("LCD Size", typeof(double));
            dtPointBorda.Columns.Add("RAM Capacity", typeof(double));
            dtPointBorda.Columns.Add("Memory Capacity", typeof(double));
            dtPointBorda.Columns.Add("Processor Speed", typeof(double));
            dtPointBorda.Columns.Add("Price", typeof(double));
            dtPointBorda.Columns.Add("Total_Rows", typeof(double));
            dtPointBorda.Columns.Add("Borda_Point", typeof(double));

            DataTable dtAllSurvey = new DataTable();
            string queryAllSurvey = @"
                    Select Main_Priority, Second_Priority, Third_Priority, Fourth_Priority, Fifth_Priority, COUNT(Main_Priority) as Qty
	                FROM Data_Survey
	                group by Main_Priority, Second_Priority, Third_Priority, Fourth_Priority, Fifth_Priority
	                order by Main_Priority asc";
            string sErrorAllSurvey = BaseProgram.GetDataTable("", queryAllSurvey, new Dictionary<string, object>(), out dtAllSurvey);


            DataTable dtSubjectBorda = new DataTable();
            string querySubjectBorda = "Select * FROM Borda_Subject_List";
            string sErrorSubjectBorda = BaseProgram.GetDataTable("", querySubjectBorda, new Dictionary<string, object>(), out dtSubjectBorda);

            foreach (DataRow row in dtSubjectBorda.Rows)
            {
                double totalPoint = 0;
                DataRow newPointBorda = dtPointBorda.NewRow();
                newPointBorda["Category"] = row["Subject_Desc"].ToString();
                foreach (DataColumn col in dtPointBorda.Columns)
                {
                    if (col.ColumnName == "Category" || col.ColumnName == "Total_Rows")
                        continue;

                    if (col.ColumnName == "Borda_Point")
                    {
                        newPointBorda[col.ColumnName] = 0;
                        continue;
                    }

                    if (newPointBorda["Category"].ToString() == col.ColumnName)
                    {
                        newPointBorda[col.ColumnName] = 0;
                    }
                    else
                    {
                        double sumPoint = 0;
                        DataRow[] tempPoint = dtAllSurvey.AsEnumerable()
                            .Where(r =>
                                r.Field<string>("Main_Priority") == newPointBorda["Category"].ToString() ||
                                (
                                    r.Field<string>("Second_Priority") == newPointBorda["Category"].ToString() &&
                                    (
                                        r.Field<string>("Third_Priority") == col.ColumnName ||
                                        r.Field<string>("Fourth_Priority") == col.ColumnName ||
                                        r.Field<string>("Fifth_Priority") == col.ColumnName

                                    )
                                ) ||
                                (
                                    r.Field<string>("Third_Priority") == newPointBorda["Category"].ToString() &&
                                    (
                                        r.Field<string>("Fourth_Priority") == col.ColumnName ||
                                        r.Field<string>("Fifth_Priority") == col.ColumnName

                                    )
                                ) ||
                                (
                                    r.Field<string>("Fourth_Priority") == newPointBorda["Category"].ToString() &&
                                    (
                                        r.Field<string>("Fifth_Priority") == col.ColumnName
                                    )
                                )
                            ).ToArray();

                        if (tempPoint != null && tempPoint.Length > 0)
                        {
                            sumPoint = tempPoint.Sum(r => r.Field<int>("Qty"));
                            totalPoint += sumPoint;
                        }
                        newPointBorda[col.ColumnName] = sumPoint;
                    }
                }
                newPointBorda["Total_Rows"] = totalPoint;
                dtPointBorda.Rows.Add(newPointBorda);
            }
            return dtPointBorda;
        }
    }
}