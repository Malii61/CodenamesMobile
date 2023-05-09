using UnityEngine;
using TMPro;
public class UsernameUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    private const string PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER = "PlayerNameMultiplayer";
    public static string username;
    private void Awake()
    {
        username = PlayerPrefs.GetString(PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER, "Oyuncu" + UnityEngine.Random.Range(100, 1000));
        inputField.text = username;
    }
    public void OnInputFieldTextChange()
    {
        username = inputField.text;
        PlayerPrefs.SetString(PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER, username);
    }
}
