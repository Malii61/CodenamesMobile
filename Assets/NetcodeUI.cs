using UnityEngine;
using UnityEngine.UI;
public class NetcodeUI : MonoBehaviour
{
    [SerializeField] private Button startHostButton;
    [SerializeField] private Button startClientButton;
    private void Start()
    {
        startHostButton.onClick.AddListener(() =>
        {
            CodenamesGameMultiplayer.Instance.StartHost();
            Loader.LoadScene(Loader.Scene.GameScene);
            gameObject.SetActive(false);
        });
        startClientButton.onClick.AddListener(() =>
        {
            CodenamesGameMultiplayer.Instance.StartClient();
            gameObject.SetActive(false);
        });
    }
}
