using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour {

    public Transform SpawnTran;
    public Transform ProjectilePrefab;

    //private

    GameManager GM;
    Transform currentBall;
    
    void Start()
    {
        GM = GameManager.singleton;

        Refill();
    }

    void Update() {
        currentBall.position = SpawnTran.position;
    }

    public void ShootBall() {
        currentBall.GetComponent<ProjectileBall>().ActivateBall(currentBall.position);

        Refill();
    }

    void Refill () {
        currentBall = Instantiate(ProjectilePrefab);

        currentBall.transform.position = SpawnTran.position;

        Color randColor = GM.BallColors[Random.Range(0, GM.BallColors.Count)];

        currentBall.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", randColor);
        currentBall.GetComponent<ProjectileBall>().BallColor = randColor;
    }
}
