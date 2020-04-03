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

    public float curveInterval = 0.1f;

    void OnValidate () {
        if (curveInterval <= 0) {
            curveInterval = 0.001f;
        } else if (curveInterval >= 0.5) {
            curveInterval = 0.5f;
        }
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
