using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WordGenerator : MonoBehaviour
{
    private static List<string> Words = new List<string>()
    {
        "Ay",
        "Güneþ",
        "Festival",
        "Doða",
        "Silgi",
        "Orangutan",
        "Aslan",
        "Ev",
        "Hýrsýz",
        "Olay",
        "Klavye",
        "Kulaklýk",
        "Siyah",
        "Mat",
        "Kale",
        "Ýstanbul",
        "Avustralya",
        "Süt",
        "Kedi",
        "Ekmek",
        "Çikolata",
        "Tuþ",
        "Güreþ",
        "Uyluk",
        "Köprücük",
        "Saðlýk",
        "Sancak",
        "Fetih",
        "Flüt",
        "Gitar",
        "Pazar",
        "Devlet",
        "Çanta",
        "Rövanþ",
        "Salý",
        "Galatasaray",
        "Trabzonspor",
        "Düdük",
        "Seçim",
        "Bayrak",
        "Namaz",
        "Tesbih",
        "Zemzem",
        "Hukuk",
        "Terazi",
        "Oduncu",
        "Þubat",
        "Ocak",
        "Karadeniz",
        "Kral",
        "Festival",
        "Paraþüt",
        "Kare",
        "Simit",
        "Göbek",
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
