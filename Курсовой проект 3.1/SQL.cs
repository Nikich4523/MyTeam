using System;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Windows;

namespace Курсовой_проект_3._1
{
    class SQL
    {
        string connectionString;
        public string connectionName { get; set; }

        public SQL(string connectionName)
        {
            this.connectionName = connectionName;
            connectionString = ConfigurationManager.ConnectionStrings[connectionName].ConnectionString;
        }

        public DataTable SendSelectQuery(string sqlQuery)
        {
            DataTable dataTable = new DataTable();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();

                SqlCommand command = new SqlCommand(sqlQuery, connection);


                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(dataTable);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }

            return dataTable;
        }
    }
}
