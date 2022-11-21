using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateFile : MonoBehaviour
{


    public static GenerateFile Instance;
    string boardTxt;
    int winGameTxt;

    private void Awake()
    {
        Instance = this;
    }

    public void generateFile(string board)
    {
        this.boardTxt = board;

    }

    public void generateWinGame(int winGameTxt)
    {
        this.winGameTxt = winGameTxt;
    }

    public void GenerateTxt()
    {

        string fileName = @"D:\Unity Games\topDown2D\txt\Game.txt";
        string lastGame = "";

        try
        {
            // Check if file already exists. If yes, delete it.     
            if (System.IO.File.Exists(fileName))
            {
                using (System.IO.StreamReader sr = System.IO.File.OpenText(fileName))
                {
                    string s = "<?xml version='1.0' encoding='utf-8'?> \n";
                    s = s + "<data>";
                    while ((s = sr.ReadLine()) != null)
                    {
                       lastGame = lastGame + s + "\n";
                    }
                    lastGame = lastGame.Split("</data>")[0];
                }
                System.IO.File.Delete(fileName);
            }else{
                lastGame = "<?xml version='1.0' encoding='utf-8'?> \n";
                lastGame = lastGame +  "<data>";
            }

            // Create a new file     
            using (System.IO.FileStream fs = System.IO.File.Create(fileName))
            {
                // Add some text to file    
                byte[] title = new System.Text.UTF8Encoding(true).GetBytes(lastGame + this.boardTxt + "<winner>" + this.winGameTxt + "</winner></row></data>");
                fs.Write(title, 0, title.Length);
            }
        }
        catch (System.Exception Ex)
        {
            System.Console.WriteLine(Ex.ToString());
        }

    }
}
