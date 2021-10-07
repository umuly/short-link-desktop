using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Operations
{
    public static class OpSQLite
    {
        public static void Read(string connectionString)
        {
            //string conn = "URI=file:" + Application.dataPath + "/shortlinkdb.db"; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(connectionString);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "SELECT * " + "FROM token";
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                string token = reader.GetString(0);

                Debug.Log("Token = " + token);
            }
            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
        }
    }
}
