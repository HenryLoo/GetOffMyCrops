using System;
using System.Data;

public interface IDatabase
{
    // Connect to the database and return if connection was successful
    bool Connect( string dbPath );

    // Perform a query on the database and return the reader with the result
    IDataReader Query( string queryString );

    // Perform necessary tasks to close the connection to the database
    // Return true if there were no issues
    bool Close();
}
