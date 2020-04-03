using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movingBalls : MonoBehaviour {

    public Color MyColor;

    public float MoveSpeed = 5f;

    private bool isMoving = false;

    private bool hasHit = false;

    private ballManager bm;

    void OnEnable () {
        bm = ballManager.singleton;
    }

    void FixedUpdate() {
        if (!isMoving || hasHit) {
            return;
        }

        transform.position += transform.forward * MoveSpeed * Time.fixedDeltaTime;
        Collider[] cols = Physics.OverlapSphere(transform.position, 0.5f);

        if(cols.Length != 0)    {
            if(cols[0].CompareTag("Target"))    {
                GameObject hitObj = cols[0].gameObject;
                int index = bm.balls.IndexOf(hitObj);
                
                if (hitObj.transform.position.x >= transform.position.x) {
                    index++;
                }
                
                bm.AddBall(index, MyColor);
                hasHit = true;//!temporary

                Destroy(gameObject);
            }
        }
    }

    public void ActivateBall () {
        isMoving = true;
    }
}
