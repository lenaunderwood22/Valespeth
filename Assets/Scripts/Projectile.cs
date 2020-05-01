using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public int BallColorID;

    GameManager gm;

    bool isActive = false;

    float speed = 0;

    float rotationSpeed = 200;


    Vector3 currentEulerAngles;
    Quaternion currentRotation;

    Collider lastCollider;
    Vector3 lastHitPoint;

    void OnEnable () {
        gm = GameManager.singleton;
    }

    public void ActivateProjectile (Vector3 position, float _speed) {
        isActive = true;

        transform.position = position;

        speed = _speed;

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

        currentEulerAngles += new Vector3(0.0f, 0.0f, -3.0f)  * Time.deltaTime * rotationSpeed;
        currentRotation.eulerAngles = currentEulerAngles;
        transform.rotation = currentRotation;
        
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