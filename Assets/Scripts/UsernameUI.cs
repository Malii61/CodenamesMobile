using UnityEngine;
using TMPro;
public class UsernameUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    public static string username;
    private void Start()
    {
        username = "Player" + Random.Range(10, 991);
        inputField.text = username;
    }
    public void OnInputFieldTextChange()
    {
        username = inputField.text;
    }
}
