using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public int BallColorID;

    GameManager gm;

    bool isActive = false;

    float speed = 0;

    Collider lastCollider;
    Vector3 lastHitPoint;

    void OnEnable () {
        gm = GameManager.singleton;
    }

    public void ActivateProjectile (Vector3 position, float _speed) {
        isActive = true;

        transform.position = position;

        speed = _speed;

        GetComponent<TrailRenderer>().enabled=true; 
        GetComponent<TrailRenderer>().startColor = (gm.BallColors[BallColorID] * gm.ColorEmissionIntensity);
        // GetComponent<TrailRenderer>().endColor = (gm.BallColors[BallColorID] * (gm.ColorEmissionIntensity));

        Destroy(gameObject, 10f);
    }

    void FixedUpdate () {
        if (!isActive) {
            return;
        }

        RaycastHit hit;

        if (Physics.SphereCast(transform.position, gm.BallDiameter * 0.5f, transform.forward, out hit, 100f, gm.RollerLayer)) {
            lastCollider = hit.collider;
            lastHitPoint = hit.point;
        }
    }

    void Update () {
        if (!isActive) {
            return;
        }

        transform.position += transform.forward * speed * Time.deltaTime;
        
        if (lastCollider != null) {
            if (Vector3.Distance(transform.position, lastCollider.transform.position) <= gm.BallDiameter) {
                RollerCollision(lastCollider.GetComponent<Roller>(), lastHitPoint);
            }
        }
    }

    void RollerCollision (Roller hitRoller, Vector3 hitPoint) {
        Chain hitChain = hitRoller.chain;

        int atIndex = hitChain.GetIndexOfRoller(hitRoller);

        if (transform.position.x < hitRoller.Tran.position.x) {
            atIndex++;
        }

        hitChain.InsertRollerAt(atIndex, BallColorID, hitPoint);

        isActive = false;
        Destroy(gameObject);
    }

}