using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ClassLibTeam12.Data.Framework
{
    class SqlServer
    {
        SqlConnection connection;
        SqlDataAdapter adapter;
        public SqlServer()
        {
            connection = new SqlConnection(Settings.GetConnectionString());
        }
        protected SelectResult Select(SqlCommand selectCommand)
        {
            var result = new SelectResult();
            try
            {
                using (connection)
                {
                    selectCommand.Connection = connection;
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();
                    adapter = new SqlDataAdapter(selectCommand);

                    result.DataTable = new System.Data.DataTable();
                    adapter.Fill(result.DataTable);
                    connection.Close();
                }
                result.Succeeded = true;
            }
            catch (Exception ex)
            {
                result.AddError(ex.Message);
            }
            return result;
        }
        protected SelectResult Select(string tableName)
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = $"SELECT * FROM {tableName}";
            return Select(command);
        }

        protected InsertResult InsertRecord(SqlCommand insertCommand)
        {
            InsertResult result = new InsertResult();
            try
            {
                using (connection)
                {
                    insertCommand.CommandText += "SET @new_id = SCOPE_IDENTITY();";
                    insertCommand.Parameters.Add("@new_id", SqlDbType.Int).Direction =
                    ParameterDirection.Output;
                    insertCommand.Connection = connection;
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();
                    insertCommand.ExecuteNonQuery();
                    int newId = Convert.ToInt32(insertCommand.Parameters["@new_id"].Value);
                    result.NewId = newId;
                    connection.Close();
                }
                result.Succeeded = true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        protected ResetResult UpdateRecord(SqlCommand updateCommand)
        {

            ResetResult result = new ResetResult();
            try
            {
                using (connection)
                {
                    updateCommand.Connection = connection;
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();
                    int rowsAffected = updateCommand.ExecuteNonQuery();
                    result.RowsAffected = rowsAffected;
                    connection.Close();
                }
                result.Succeeded = true;
            }
            catch (Exception ex)
            {
                result.AddError(ex.Message);
            }
            return result;
        }

        protected SelectResult ExecuteSelect(string query, Dictionary<string, object> parameters)
        {
            var result = new SelectResult
            {
                DataTable = new DataTable()
            };
            try
            {
                using (var connection = new SqlConnection(Settings.GetConnectionString()))
                using (var command = new SqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            command.Parameters.AddWithValue(param.Key, param.Value);
                        }
                    }

                    connection.Open();
                    using (var adapter = new SqlDataAdapter(command))
                    {
                        result.DataTable = new DataTable();
                        adapter.Fill(result.DataTable);
                    }
                    connection.Close();
                }
                result.Succeeded = true;
            }
            catch (Exception ex)
            {
                result.AddError(ex.Message);
            }
            return result;
        }
        protected DeleteResult DeleteRecords(SqlCommand deleteCommand)
        {
            DeleteResult result = new DeleteResult();
            try
            {
                using (connection)
                {
                    deleteCommand.Connection = connection;
                    connection.Open();
                    int rowsAffected = deleteCommand.ExecuteNonQuery();
                    result.RowsAffected = rowsAffected;
                    result.Succeeded = rowsAffected > 0;
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Delete failed: " + ex.Message, ex);
            }
            return result;
        }
    }
}
