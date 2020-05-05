using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject StartPanel;
    public GameObject LevelsPanel;

    void Awake () {
        ToStartScreen();
    }

    public void QuitGame () {
        Application.Quit();
    }

    public void LoadLevel (int buildIndex) {
        SceneManager.LoadScene(buildIndex);
    }

    public void ToStartScreen () {
        StartPanel.SetActive(true);
        LevelsPanel.SetActive(false);
    }

    public void ToLevelsScreen () {
        StartPanel.SetActive(false);
        LevelsPanel.SetActive(true);
    }
}
