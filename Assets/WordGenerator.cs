using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WordGenerator : MonoBehaviour
{
    private static List<string> Words = new List<string>()
    {
        "Ay",
        "G�ne�",
        "Festival",
        "Do�a",
        "Silgi",
        "Orangutan",
        "Kaplan",
        "Ev",
        "H�rs�z",
        "Olay",
        "Klavye",
        "Kulakl�k",
        "Siyah",
        "Mat",
        "Kale",
        "�stanbul",
        "Avustralya",
        "S�t",
        "Kedi",
        "Ekmek",
        "�ikolata",
        "Tu�",
        "G�re�",
        "Uyluk",
        "K�pr�c�k",
        "Sa�l�k",
        "Sancak",
        "Fetih",
        "Fl�t",
        "Gitar",
    };

    public static List<string> GenerateRandomWordList(int listLength)
    {
        System.Random rng = new System.Random();
        List<string> shuffledList = Words.OrderBy(a => rng.Next()).ToList();
        if (listLength >= Words.Count)
        {
            return shuffledList;
        }

        return shuffledList.GetRange(0, listLength);
    }
}
/*2:55:30 te t�m oyuncular�n haz�r oldu�u bilgisini i�eren kod
 
 
 
 
 
 
 */