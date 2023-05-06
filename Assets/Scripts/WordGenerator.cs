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
        "Aslan",
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
        "Pazar",
        "Devlet",
        "�anta",
        "R�van�",
        "Sal�",
        "Galatasaray",
        "Trabzonspor",
        "D�d�k",
        "Se�im",
        "Bayrak",
        "Namaz",
        "Tesbih",
        "Zemzem",
        "Hukuk",
        "Terazi",
        "Oduncu",
        "�ubat",
        "Ocak",
        "Karadeniz",
        "Kral",
        "Festival",
        "Para��t",
        "Kare",
        "Simit",
        "G�bek",
        "Makas",
        "Taht",
        "Hortum",
        "Lazer",
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
