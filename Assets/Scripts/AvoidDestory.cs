using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class AvoidDestory : MonoBehaviour
{
    void Awake(){
        if(SceneManager.GetActiveScene().name.Equals("TitleScene") ){
        DontDestroyOnLoad(transform.gameObject);
        }
    }
}
