using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{

    public GameObject PausePanel;

    void Start(){
        PausePanel.SetActive(false);
    }
    public void PauseGame(){
        PausePanel.SetActive(true);
        Time.timeScale = 0f ;
    }

    public void ResumeGame(){
        PausePanel.SetActive(false);
        Time.timeScale = 1f ;
    }

    public void QuitGame(){
        Application.Quit();
    }

    public void BackToMain(){
        PausePanel.SetActive(false);
        Time.timeScale = 1f ;
        SceneManager.LoadScene("TitleScene");
    }
}
