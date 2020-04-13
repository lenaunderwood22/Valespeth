using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneButton : MonoBehaviour
{
    public void BtnNewScene(string scene_name)
    {
        SceneManager.LoadScene(scene_name);
    }
}
