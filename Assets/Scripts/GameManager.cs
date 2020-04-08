using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    #region init
    public static GameManager singleton = null;

    void Awake () {
        if (singleton == null) {
            singleton = this;
        } else {
            Debug.LogError("GameManager Already Exists!");
            Destroy(this);
        }
    }
    #endregion

    public GameObject RollerPrefab;
    public List<Color> BallColors;
    
    public Shooter Shooter;

    public float ShootRate;

    public float ProjectileMoveSpeed = 5f;

    public float RateByDistance = 1f;
    public float RollingSpeed = 1.5f;

    public float TimerDelay = 2f;

    //private
    
    float shootTimer;

    void Start () {        
        shootTimer = 0;
    }

    void Update () {
        if(shootTimer >= ShootRate && Input.GetKeyDown(KeyCode.Space)) {
            shootTimer = 0;

            Shooter.ShootBall();
        }

        shootTimer += Time.deltaTime;
    }

    public static Vector3 CubicInterp (Transform pA, Transform pB, float t) {
        Vector3 A = pA.position;
        Vector3 B = pA.position + pA.forward * pA.localScale.z;
        Vector3 C = pB.position - pB.forward * pB.localScale.z;
        Vector3 D = pB.position;

        return (1-t)*(1-t)*(1-t)*A + 3*(1-t)*(1-t)*t*B + 3*(1-t)*t*t*C + t*t*t*D;
    }

    public static Vector3 CubicInterpVisual (Transform pA, Transform pB, float t) {
        Vector3 A = pA.position;
        Vector3 B = pA.position + pA.forward * pA.localScale.z;
        Vector3 C = pB.position - pB.forward * pB.localScale.z;
        Vector3 D = pB.position;

        var lAB = Vector3.Lerp(A, B, t);
        var lBC = Vector3.Lerp(B, C, t);
        var lCD = Vector3.Lerp(C, D, t);

        var l1 = Vector3.Lerp(lAB, lBC, t);
        var l2 = Vector3.Lerp(lBC, lCD, t);

        var l = Vector3.Lerp(l1, l2, t);

        return l;
    }
}
