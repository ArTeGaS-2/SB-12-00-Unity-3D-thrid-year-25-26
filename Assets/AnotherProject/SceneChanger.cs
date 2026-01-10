using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void ToGame()
    {
        SceneManager.LoadScene("EndlessMadness_game");
    }
    public void ToMenu()
    {
        SceneManager.LoadScene("EndlessMadness_menu");
    }
}
