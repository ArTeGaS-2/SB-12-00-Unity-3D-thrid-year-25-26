using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyReturnToMenuOnPlayerContact : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private string menuSceneName = "MainMenu";

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag))
        {
            return;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(menuSceneName);
    }
}
