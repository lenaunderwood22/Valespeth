using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ballShooter : MonoBehaviour
{
    public ballManager BallManager;

    public Transform spawnTran;
    public Transform ballPrefab;

    Transform currentBall;
    
    void Start()
    {
        AddNewBall();
    }

    void Update()
    {
        currentBall.position = spawnTran.position;
    }

    public void shoot() {
        currentBall.GetComponent<movingBalls>().ActivateBall();

        AddNewBall();
    }

    void AddNewBall () {
        currentBall = Instantiate(ballPrefab);

        Color randColor = BallManager.ColorList[Random.Range(0, BallManager.ColorList.Count)];

        currentBall.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", randColor);        
        currentBall.GetComponent<movingBalls>().MyColor = randColor;
    }
}
