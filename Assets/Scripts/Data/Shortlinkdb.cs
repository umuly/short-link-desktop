using Mono.Data.Sqlite;
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

        public static T Test { get; set; }

        public List<T> Lists { get; set; } 

        public List<T> Que(string sqlQuery ) {
            dbconn.Open();
            IDbCommand dbcmd = dbconn.CreateCommand();
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();
            Lists = new List<T>();
            
            while (reader.Read())
            {

                foreach (var item in typeof(T).GetProperties())
                {
                    item.SetValue(Test, Convert.ChangeType(reader[item.Name], item.PropertyType));
                }

                Lists.Add(Test);
            }
            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;

            return Lists;

        }
    }
}
