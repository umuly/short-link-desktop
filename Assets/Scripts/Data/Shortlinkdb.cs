﻿using Mono.Data.Sqlite;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Data
{
    public class Shortlinkdb<T>
    {
        IDbConnection dbconn;
        public Shortlinkdb()
        {
            string conn = "URI=file:" + Application.dataPath + "/shortlinkdb.db"; //Path to database.
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
    }
}