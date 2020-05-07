using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{

    public GameObject PausePanel;
    public GameObject PauseButton;

    public AudioSource bgm;

    void Start(){
        PausePanel.SetActive(false); 
        PauseButton.SetActive(true);
    }
    public void PauseGame(){
        PausePanel.SetActive(true); 
        PauseButton.SetActive(false); 
        Time.timeScale = 0f ;
        bgm.volume = bgm.volume / 2;
    }

    public void ResumeGame(){
        PauseButton.SetActive(true); 
        PausePanel.SetActive(false); 
        Time.timeScale = 1f ;
        bgm.volume = bgm.volume * 2;
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
