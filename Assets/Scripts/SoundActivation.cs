using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundActivation : MonoBehaviour
{
    public AudioSource sound;
    public AudioSource bgm;
    
    void Start(){
        sound.Play();
        bgm.Pause();
    }
}
