using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class MainMenuPlayButton : MonoBehaviour
{
    [SerializeField] private string sceneName = "GameInstance";

    private void Awake()
    {
        var button = GetComponent<Button>();
        if (button == null)
        {
            Debug.LogError("MainMenuPlayButton requires a Button component.");
            return;
        }

        button.onClick.AddListener(LoadScene);
    }

    private void LoadScene()
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Scene name is not set for MainMenuPlayButton.");
            return;
        }

        SceneManager.LoadScene(sceneName);
    }
}
