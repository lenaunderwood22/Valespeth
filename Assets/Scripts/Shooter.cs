using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour {

    [SerializeField] Transform SpawnPoint;
    [SerializeField] Projectile ProjectilePrefab;
    [SerializeField] float ProjectileSpeed;

    Projectile curProjectile;

    GameManager gm;

    void Start () {
        gm = GameManager.singleton;

        Refill();
    }

    void Update () {
        if (curProjectile != null) {
            curProjectile.transform.position = SpawnPoint.position;
        }

    }

    public void ShootProjectile() {
        curProjectile.GetComponent<Projectile>().ActivateProjectile(curProjectile.transform.position, ProjectileSpeed);

        Refill();
    }

    void Refill () {
        curProjectile = Instantiate(ProjectilePrefab);

        curProjectile.transform.localScale = Vector3.one * gm.BallDiameter;
        
        curProjectile.transform.position = SpawnPoint.position;

        int randColorID = Random.Range(0, gm.BallColors.Count);
        Color randColor = gm.BallColors[randColorID];

        curProjectile.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", randColor * gm.ColorEmissionIntensity);
        curProjectile.GetComponent<Projectile>().BallColorID = randColorID;
    }
}