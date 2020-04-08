using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBall : MonoBehaviour {

    [HideInInspector] public Color BallColor;

    //private

    private bool isMoving = false;
    private bool hasHit = false;    

    float MoveSpeed;

    void OnEnable () {
        MoveSpeed = GameManager.singleton.ProjectileMoveSpeed;
    }

    void FixedUpdate() {
        if (!isMoving || hasHit) {
            return;
        }

        transform.position += transform.forward * MoveSpeed * Time.fixedDeltaTime;
        Collider[] cols = Physics.OverlapSphere(transform.position, 0.5f);

        if (cols.Length != 0)    {
            if (cols[0].CompareTag("Target")) {
                GameObject hitRoller = cols[0].gameObject;
                BallChain rollerChain = hitRoller.transform.root.GetComponent<BallChain>();

                int index = rollerChain.GetIndexOfRoller(hitRoller);
                
                if (hitRoller.transform.position.x >= transform.position.x) {
                    index++;
                }
                
                rollerChain.AddRoller(index, BallColor);
                rollerChain.CheckChainAt(index);
                hasHit = true;//!temporary

                Destroy(gameObject);
            }
        }
    }

    public void ActivateBall (Vector3 startingLocation) {
        transform.position = startingLocation;

        isMoving = true;
    }
}
