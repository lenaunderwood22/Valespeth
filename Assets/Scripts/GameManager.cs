using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour { 

    public static GameManager singleton = null;

    void Awake () {
        if (singleton == null) {
            singleton = this;
        } else {
            Destroy(this);
        }
    }
    
    //[ColorUsageAttribute(true,true)] 
    public List<Color> BallColors;
    public float ColorEmissionIntensity;

    [HideInInspector] public bool GameIsFinished = false;

    public float BallDiameter = 1f;
    public float FinishPointDistanceOffset = 0.5f;

    public LayerMask RollerLayer;

    public Shooter MainShooter;

    public ShooterMovement shooterMovement;

    void Update () {

        //Mobile touch
        if (Input.touchCount > 0)
        {
            Vector3 touchPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            shooterMovement.Move(touchPos.x);
            MainShooter.ShootProjectile();

            //It works if I just call MainShooter.ShootProjectile(),
            //but it doesn't work when I try to move the shooter to the x coordinate
            //of the touch position.
        }

        //Playing on computer
        if (Input.GetKeyDown(KeyCode.Space))
        {
            MainShooter.ShootProjectile();
        }
    }

    public void LoseGame () {
        Time.timeScale = 0;
        GameIsFinished = true;

        Debug.Log("Lost the Game!");
    }

    public void WinGame () {
        Time.timeScale = 0;
        GameIsFinished = true;

        Debug.Log("Won the Game!");
    }

}