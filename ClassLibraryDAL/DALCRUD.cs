using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using System.Data.SqlClient;


namespace ClassLibraryDAL
{
    public class DALCRUD
    {

        public static async Task SaveData(string ProcedureName, SqlParameter[] sqlParameters)
        {
            try
            {
                using (SqlConnection con = DBHelper.GetConnection())
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(ProcedureName, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddRange(sqlParameters);
                        await cmd.ExecuteNonQueryAsync();
                        await con.CloseAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception Occurred: {ex.Message}");
                throw; // Rethrow the exception to handle it in the Blazor component
            }
        }



        public static async Task SaveData<T>(string storedProcedureName, SqlParameter[] parameters, Action<T> processOutput = null)
        {
            try
            {
                using (SqlConnection con = DBHelper.GetConnection())
                {
                    SqlCommand cmd = new SqlCommand(storedProcedureName, con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add input parameters
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    // Add the output parameter for result
                    SqlParameter outputPara = new SqlParameter("@result", SqlDbType.NVarChar, 50)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outputPara);

                    await con.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();

                    string result = cmd.Parameters["@result"].Value.ToString();
                    Console.WriteLine($"Result: {result}");

                    // Process output if a callback is provided
                    if (processOutput != null)
                    {
                        processOutput((T)Convert.ChangeType(result, typeof(T)));
                    }

                    con.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception Occurred: {ex.Message}");
            }
        }



        public static async Task<DataTable> ReadSpecificDataTable(string ProcedureName, SqlParameter[] sqlParameters)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection conn = DBHelper.GetConnection())
                {
                    using (SqlCommand cmd = new SqlCommand(ProcedureName, conn))
                    {
                        await conn.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddRange(sqlParameters);
                        SqlDataAdapter sda = new SqlDataAdapter(cmd);
                        sda.Fill(dt);
                        SqlDataReader rdr = await cmd.ExecuteReaderAsync();
                        await conn.CloseAsync();

                        if (dt.Rows.Count > 0)
                        {

                            return dt;

                        }
                        else { return new DataTable(); }
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception Occurred: {ex.Message}");
            }
            return new DataTable();

        }

        //public static async Task<ActionResult> ReadData(string ProcedureName)
        //{
        //    DataTable dt = new DataTable();
        //    try
        //    {
        //        using (SqlConnection conn = DBHelper.GetConnection())
        //        {
        //            using (SqlCommand cmd = new SqlCommand(ProcedureName, conn))
        //            {
        //                await conn.OpenAsync();
        //                SqlDataAdapter sda = new SqlDataAdapter(cmd);
        //                sda.Fill(dt);
        //                SqlDataReader rdr = await cmd.ExecuteReaderAsync();

        //                await conn.CloseAsync();

        //                if (dt.Rows.Count > 0)
        //                {
        //                    string json = DalCustomLogics.DataTableToJSONWithJSONNet(dt);
        //                    return new ContentResult { Content = json, ContentType = "application/json" };

        //                    //this is made generic now  
        //                    //first write code for getdatatable in CRUD, then this table is send to Dalcustomlogics to get
        //                    //jsonstring result,then json string will be sent to another function to make actionresult to make string into object 
        //                }
        //                else { return new ContentResult { Content = "", ContentType = "application/json" }; }
        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Exception Occurred: {ex.Message}");
        //        return new ContentResult { Content = "", ContentType = "application/json" };

        //    }

        //}


        //public static async Task<List<ENTRegistration>> GetStudentsFromReadData(string procedureName)
        //{
        //    ActionResult result = await ReadData(procedureName);
        //    var contentResult = result as ContentResult;

        //    if (contentResult != null && !string.IsNullOrEmpty(contentResult.Content))
        //    {
        //        return JsonSerializer.Deserialize<List<ENTRegistration>>(contentResult.Content);
        //    }

        //    return new List<ENTRegistration>();
        //}


        //public static async Task<List<ENTDepartment>> GetDepartmentsFromReadData(string procedureName)
        //{
        //    ActionResult result = await ReadData(procedureName);
        //    var contentResult = result as ContentResult;

        //    if (contentResult != null && !string.IsNullOrEmpty(contentResult.Content))
        //    {
        //        return JsonSerializer.Deserialize<List<ENTDepartment>>(contentResult.Content);
        //    }

        //    return new List<ENTDepartment>();
        //}


        public static async Task<string> ReadDataAsync(string procedureName)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection conn = DBHelper.GetConnection())
                {
                    using (SqlCommand cmd = new SqlCommand(procedureName, conn))
                    {
                        await conn.OpenAsync();
                        SqlDataAdapter sda = new SqlDataAdapter(cmd);
                        sda.Fill(dt);
                        await conn.CloseAsync();

                        if (dt.Rows.Count > 0)
                        {
                            return DalCustomLogics.DataTableToJSONWithJSONNet(dt);
                        }
                        else
                        {
                            return string.Empty;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception Occurred: {ex.Message}");
                return string.Empty;
            }
        }
        public static async Task<string> ReadDataAsync(string procedureName, SqlParameter[] sp)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection conn = DBHelper.GetConnection())
                {

                    SqlCommand cmd = new SqlCommand(procedureName, conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(sp);
                    await conn.OpenAsync();
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    sda.Fill(dt);
                    await conn.CloseAsync();

                    if (dt.Rows.Count > 0)
                    {
                        return DalCustomLogics.DataTableToJSONWithJSONNet(dt);
                    }
                    else
                    {
                        return string.Empty;
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception Occurred: {ex.Message}");
                return string.Empty;
            }
        }

        public static async Task<List<T>> GetEntitiesFromReadDataAsync<T>(string procedureName)
        {
            string jsonData = await ReadDataAsync(procedureName);

            if (!string.IsNullOrEmpty(jsonData))
            {
                return JsonSerializer.Deserialize<List<T>>(jsonData);
            }

            return new List<T>();
        }
        public static async Task<List<T>> GetEntitiesFromReadDataAsync<T>(string procedureName, SqlParameter[] sp)
        {
            string jsonData = await ReadDataAsync(procedureName, sp);

            if (!string.IsNullOrEmpty(jsonData))
            {
                return JsonSerializer.Deserialize<List<T>>(jsonData);
            }

            return new List<T>();
        }
        public static async Task DeleteInfo(string procedureName, SqlParameter[] parameters)
        {
            using (SqlConnection con = DBHelper.GetConnection())
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(procedureName, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(parameters);
                    await cmd.ExecuteNonQueryAsync();
                }
                await con.CloseAsync();
            }
        }



        public static async Task UpdateInfo<T>(string procedureName, SqlParameter[] sqlParameters, string idParameterName, int id)
        {
            try
            {
                using (SqlConnection con = DBHelper.GetConnection())
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(procedureName, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue(idParameterName, id);
                        cmd.Parameters.AddRange(sqlParameters);
                        await cmd.ExecuteNonQueryAsync();
                        await con.CloseAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception Occurred: {ex.Message}");
            }
        }


    }
}