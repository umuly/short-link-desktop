using UnityEngine;
using System.Data;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using System;

namespace Assets.Scripts.Data
{
    public class ShortLinkTxtDB
    {
        public static void WriteString(string token)
        {
            string path = Application.persistentDataPath + "/token.txt";

            if (File.Exists(path))
            {
                File.WriteAllText(path, "{\"Id\":1,\"Token\":\"" + token + "\"}");
            }
            else
            {
                File.CreateText(path);
            }
        }

        public static string ReadString()
        {
            string path = Application.persistentDataPath + "/token.txt";
            //Debug.Log("Persistent Data Path: " + Application.persistentDataPath + "File Exist: " + File.Exists(Application.persistentDataPath + "/token.txt"));

            if (File.Exists(path))
            {
                //Read the text from directly from the test.txt file
                StreamReader reader = new StreamReader(path);
                string a = reader.ReadToEnd();
                var q = JsonConvert.DeserializeObject<Player>(a);
                reader.Close();
                if (q != null)
                {
                    return q.Token;
                }

                return "";
            }
            else
            {
                File.CreateText(path);
                return "";
            }
        }

        public static void ClearToken()
        {
            string path = Application.persistentDataPath + "/token.txt";

            if (File.Exists(path))
            {
                File.WriteAllText(Application.persistentDataPath + "/token.txt", String.Empty);
            }
            else
            {
                File.CreateText(path);
            }
        }
    }
}
