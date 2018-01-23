using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

using MySql.Data.MySqlClient;


public class DataAccessLayer : IDisposable
{

    MySqlConnection _connection = null;
    MySqlTransaction _transaction = null;

    public DataAccessLayer(string connectionstring)
    {
        _connection = new MySqlConnection(connectionstring);
    }

    public DataAccessLayer()
    {
        _connection = new MySqlConnection(GetAppSetting("MYCONNSTRING"));
    }

    public void Dispose()
    {
        // Close connection
        if (_connection.State != ConnectionState.Closed) { _connection.Close(); }

    }

    #region Connection Members

    // Open connection
    public void Open()
    {
        try
        {
            _connection.Open();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    // Is connection open
    public bool IsConnected
    {
        get { return (_connection != null && _connection.State != ConnectionState.Closed); }
    }

    // In transaction
    public bool InTransaction
    {
        get { return _transaction != null; }
    }

    // Close connection
    public void Close()
    {
        if (_connection != null && _connection.State == System.Data.ConnectionState.Open)
            _connection.Close();
    }

    #endregion

    #region Transaction Members

    // Begin transaction
    public bool BeginTransaction()
    {
        if (!IsConnected)
            return false;

        _transaction = _connection.BeginTransaction();

        return true;

    }

    // Commit transaction
    public bool CommitTransaction()
    {

        if (_transaction != null)
        {
            _transaction.Commit();
            _transaction = null;
            return true;
        }
        else
        {
            return false;
        }
    }

    // Rollback transaction
    public bool RollbackTransaction()
    {
        if (_transaction != null)
        {
            _transaction.Rollback();
            _transaction = null;
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion

    #region Standard Commands

    // Create db command
    private MySqlCommand CreateDbCommand(string commandText, Dictionary<string, object> parameters, CommandType cmdType)
    {
        // Create command
        MySqlCommand cmd = new MySqlCommand(commandText, _connection, _transaction);
        cmd.CommandType = cmdType;
        cmd.CommandTimeout = 0;

        // Add parameters
        if (parameters != null)
            AddDbCommandParams(cmd, parameters);

        return cmd;
    }

    // Add params (db dependant)
    private void AddDbCommandParams(MySqlCommand cmd, Dictionary<string, object> parameters)
    {

        Dictionary<string, object> newParams = new Dictionary<string, object>();

        // Render nulls
        foreach (string key in parameters.Keys)

            // Null check must be done first
            if (parameters[key] == null)
                newParams.Add(key, DBNull.Value);
            else if (parameters[key].GetType() == typeof(DateTime))
                if (Convert.ToDateTime(parameters[key]) == System.DateTime.MinValue)
                    newParams.Add(key, DBNull.Value);
                else
                    newParams.Add(key, parameters[key]);
            else
                newParams.Add(key, parameters[key]);


        // Add parameters to command
        foreach (string key in newParams.Keys)
        {
            cmd.Parameters.AddWithValue(key, newParams[key]);
        }


    }

    public int ExecuteCommandNonQuery(string commandtext, Dictionary<string, object> parameters)
    {
        int rowsAffected = 0;
        if (!IsConnected)
        {
            // Create db command
            MySqlCommand cmd = CreateDbCommand(commandtext, parameters, CommandType.Text);
            try
            {
                Open();
                // Execute command
                rowsAffected = cmd.ExecuteNonQuery();
                Close();
            }
            catch
            {
                return rowsAffected;
            }

        }
        return rowsAffected;
    }

    public object ExecuteCommandScaler(string commandtext, Dictionary<string, object> parameters)
    {
        object returnVal = null;
        if (!IsConnected)
        {
            // Create cmd
            MySqlCommand cmd = CreateDbCommand(commandtext, parameters, CommandType.Text);
            try
            {
                Open();
                // Return scaler result
                returnVal = cmd.ExecuteScalar();
                Close();
            }
            catch (Exception ex)
            {
                return returnVal;
            }

        }
        return returnVal;
    }

    public DataTable GetDataTable(string commandtext, Dictionary<string, object> parameters)
    {
        if (!IsConnected)
        {
            // Create cmd
            MySqlCommand cmd = CreateDbCommand(commandtext, parameters, CommandType.Text);

            // Prepare adapter
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            // Fill dataTable
            DataTable dataTab = new DataTable();
            try
            {
                adapter.Fill(dataTab);
                return dataTab;
            }
            catch
            {
                return null;
            }
        }
        return null;
    }

    public DataTable GetDataTableFromSP(string commandtext, Dictionary<string, object> parameters)
    {
        if (!IsConnected)
        {
            // Create command
            MySqlCommand cmd = CreateDbCommand(commandtext, parameters, CommandType.StoredProcedure);

            // Prepare adapter
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            // Fill dataTable
            DataTable dataTab = new DataTable();
            try
            {
                adapter.Fill(dataTab);
                return dataTab;
            }
            catch (Exception Ex)
            {
                return null;
            }
        }
        return null;
    }

    public DataSet GetDataTablesFromSP(string commandtext, Dictionary<string, object> parameters)
    {
        if (!IsConnected)
        {
            // Create command
            MySqlCommand cmd = CreateDbCommand(commandtext, parameters, CommandType.StoredProcedure);

            // Prepare adapter
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            // Fill dataTable
            DataSet dataSet = new DataSet();
            try
            {
                adapter.Fill(dataSet);
                return dataSet;
            }
            catch (Exception Ex)
            {
                return null;
            }
        }
        return null;
    }

    public DataSet GetDataSetFromSP(string commandtext, Dictionary<string, object> parameters)
    {
        if (!IsConnected)
        {
            // Create command
            MySqlCommand cmd = CreateDbCommand(commandtext, parameters, CommandType.StoredProcedure);

            // Prepare adapter
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            // Fill dataTable
            DataSet dataset = new DataSet();
            try
            {
                adapter.Fill(dataset);
                return dataset;
            }
            catch
            {
                return null;
            }
        }
        return null;
    }

    #endregion

    public interface IDispose
    {
        void Dispose();
    }

    private string GetAppSetting(string name)
    {
        try
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }
        catch
        {
            //try
            //{
            //    return Settings.Default.MainConnectionString;
            //    return "";
            //}
            //catch { return ""; }
            return null;
        }
    }


}
