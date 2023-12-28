using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParserJson
{


    public static string ParseStringToJson(string text , string param)
    {
        string pivot = "";
        for(int i = 0; i < text.Length; i++)
        {
            pivot += text[i];
            if (pivot.Contains(param))
            {
                int j = i + 4;
                string stringReturn = "";
                do
                {
                    if(text[j].Equals('\"'))
                            return stringReturn;
                    stringReturn += text[j];
                    j++;
                    Debug.Log(j + " " + text[j]) ;
                } while (!text[j].Equals('\"') && j < 300);
                return stringReturn;
            }
        }

        return "";
    }

    public static string ParseStringToJsonWithoutCote(string text, string param)
    {
        string pivot = "";
        for (int i = 0; i < text.Length; i++)
        {
            pivot += text[i];
            if (pivot.Contains(param))
            {
                int j = i + 3;
                string stringReturn = "";
                do
                {
                    if (text[j].Equals('}'))
                        return stringReturn;
                    stringReturn += text[j];
                    j++;
                    Debug.Log(j + " " + text[j]);
                } while (!text[j].Equals('}') && j < 300);
                return stringReturn;
            }
        }

        return "";
    }
}
