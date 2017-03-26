using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class FileReader { 

    public static string[][] readFile(string file)
    {
        string text = System.IO.File.ReadAllText(file);
        text = text.Replace("[", "");
        string[] lines = Regex.Split(text, "],");
        int rows = lines.Length;

        string[][] levelBase = new string[rows][];
        for (int i = 0; i < lines.Length; i++)
        {
            string[] stringsOfLine = Regex.Split(lines[i], ",");
            levelBase[i] = stringsOfLine;
        }
        return levelBase;
    }


}
