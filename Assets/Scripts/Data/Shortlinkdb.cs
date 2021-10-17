using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Data
{
    public class Shortlinkdb<T>
    {
        readonly IDbConnection dbconn;
        public Shortlinkdb()
        {
            string conn = "URI=file:" + Application.persistentDataPath + "/shortlinkdb.db"; //Path to database.
            dbconn = (IDbConnection)new SqliteConnection(conn);
        }

        public T Test { get; set; }

        public List<T> Lists { get; set; } 

        public List<T> Que(string sqlQuery ) {
            dbconn.Open();
            IDbCommand dbcmd = dbconn.CreateCommand();
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();
            Lists = new List<T>();
            
            while (reader.Read())
            {

                Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
                foreach (var property in typeof(T).GetProperties())
                {
                    string value = reader[property.Name].ToString();
                    keyValuePairs.Add(property.Name, value);
                }

                string json = "{" + string.Join(",", keyValuePairs.Select(k => "\"" + k.Key + "\":\"" + k.Value + "\"")) + "}";
                var data  =  JsonConvert.DeserializeObject<T>(json);
                Lists.Add(data);
            }
            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            

            return Lists;

        }

        public void Insert(string sqlQuery)
        {
            dbconn.Open();
            IDbCommand dbcmd = dbconn.CreateCommand();
            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteNonQuery();
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            

        }

        public void Update(string sqlQuery)
        {
            dbconn.Open();
            IDbCommand dbcmd = dbconn.CreateCommand();
            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteNonQuery();
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();


        }

        public void Delete(string sqlQuery)
        {
            dbconn.Open();
            IDbCommand dbcmd = dbconn.CreateCommand();
            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteNonQuery();
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            

        }

        public void CreateTable(string sqlQuery)
        {
            dbconn.Open();
            IDbCommand dbcmd = dbconn.CreateCommand();
            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteNonQuery();
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();


        }
    }
}
