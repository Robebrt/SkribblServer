using MySqlConnector;
using System;
using System.Collections.Generic;

public class SkribblDbConnection
{
    private MySqlConnection connection;

    //Constructor
    public SkribblDbConnection(string server, string database, string uid, string password)
    {
        string connectionString;
        connectionString = $"SERVER={server};DATABASE={database};UID={uid};PASSWORD={password};";

        connection = new MySqlConnection(connectionString);
    }

    //Open connection to the database
    private bool OpenConnection()
    {
        try
        {
            connection.Open();
            return true;
        }
        catch (MySqlException ex)
        {
            //When handling errors, you can display the error number to the console and handle each case separately
            switch (ex.Number)
            {
                case 0:
                    Console.WriteLine("Cannot connect to server.");
                    break;

                case 1045:
                    Console.WriteLine("Invalid username/password, please try again.");
                    break;
            }
            return false;
        }
    }

    //Close connection
    private bool CloseConnection()
    {
        try
        {
            connection.Close();
            return true;
        }
        catch (MySqlException ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }

    //Select all words from the words table
    public string[] GetAllWords()
    {
        string query = "SELECT word FROM WordsTable";

        //Create a list to store the result
        List<string> words = new List<string>();

        //Open connection
        if (this.OpenConnection() == true)
        {
            //Create Command
            MySqlCommand cmd = new MySqlCommand(query, connection);
            //Create a data reader and Execute the command
            MySqlDataReader dataReader = cmd.ExecuteReader();

            //Read the data and store them in the list
            while (dataReader.Read())
            {
                words.Add(dataReader["word"] + "");
            }

            //close Data Reader
            dataReader.Close();

            //close Connection
            this.CloseConnection();

            //return list to be displayed
            return words.ToArray();
        }
        else
        {
            return new string[0];
        }
    }
}