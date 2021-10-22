using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class CVSParser : MonoBehaviour
{
    static private List<string> languageList = new List<string>();
    static private Dictionary<string, List<string>> languageDictionary = new Dictionary<string, List<string>>();

    static public string[] Splitline(string line)
    {
        return (from System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches(input: line,
            pattern: @"(((?<x> (?= [, \r\n]+)) |""(?<x> ([^"""""")+)"" | (?<x> [^,\r\n]+)),?)",
            System.Text.RegularExpressions.RegexOptions.ExplicitCapture)
                select m.Groups[1].Value).ToArray();
    }

    internal static string GetTextfromId(int index, string key)
    {
        throw new NotImplementedException();
    }

    static public List<string> GetAvailableLanguages()
    {
        if (languageList.Count == 0)

        {
            var cvsFile = Resources.Load<TextAsset>(path: "Localization");
            string[] lines = cvsFile.text.Split("\n"[0]);
            languageList = new List<string>(collection: Splitline(lines[0]));
            languageList.RemoveAt(index: 0);
        }
        return languageList;
    }
    static public string GetTextFromId(int languageIndex,string Id)
    {

        if (languageDictionary.Count == 0)
        {
            var cvsFile = Resources.Load<TextAsset>(path: "Localization");

            string[] lines = cvsFile.text.Split("\n"[0]);

            for (int i = 1; i < lines.Length; ++i)
            {
                string[] row = Splitline(lines[i]);

                if (row.Length > 1)
                {
                    List<string> worlds = new List<string>(row);
                    worlds.RemoveAt(index: 0);
                    languageDictionary.Add(row[0], worlds);
                }


                
                
              



            }
        }
        var values = languageDictionary[Id];
        return values[languageIndex];
    }
}
        
