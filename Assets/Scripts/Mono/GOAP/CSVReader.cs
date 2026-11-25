using System.Collections.Generic;
using UnityEngine;

public class CSVReader 
{

    public static List<Dictionary<string, string>> ReadCSV(
        TextAsset file)
    {
        // Reads CSV and returns header-keyed rows
        if (file == null)
        {

            return new(); 
        }

        List<List<string>> rows = ParseCSV(file.text);
        List<Dictionary<string, string>> rowDictionary = new();

        if (rows.Count == 0)
        {

            return rowDictionary;
        }

        for (int i = 1; i < rows.Count; i++)
        {

            Dictionary<string, string> currentRowDiciontary = new();

            List<string> headers = rows[0];

            if (rows[i].Count == 1 && rows[i][0] == "") continue;

            if (rows[i].Count != headers.Count) continue; 

            for (int j = 0; j < rows[i].Count; j++)
            {

                string currentHeader = rows[0][j];

                currentRowDiciontary.Add(currentHeader, rows[i][j]); 

            }

            rowDictionary.Add(currentRowDiciontary); 
            
        }

        return rowDictionary; 

    }

    private static List<List<string>> ParseCSV(string text)
    {
        // Handles rows, commas, quotes and line endings
        List<List<string>> rows = new List<List<string>>();
        List<string> currentRow = new List<string>();

        string currentCell = "";

        foreach (var c in text.ToCharArray())
        {

            if (c == ',')
            {

                currentRow.Add(currentCell);
                currentCell = "";

            }
            else if (c == '\n')
            {

                currentRow.Add(currentCell);
                rows.Add(currentRow);

                currentRow = new List<string>();
                currentCell = "";

            }
            else if (c == '\r')
            {
                continue;
            }
            else currentCell += c; 

        }

        if (currentCell.Length > 0 || currentRow.Count > 0)
        {

            currentRow.Add(currentCell);
            rows.Add(currentRow); 

        }

        return rows; 

    }

}
