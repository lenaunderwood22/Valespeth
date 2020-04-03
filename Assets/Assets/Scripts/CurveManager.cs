using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurveManager : MonoBehaviour {

    public Curve MainCurve;

    public Transform PivotA;
    public Transform PivotB;
    public Transform PivotC;

    public Transform Tester;

    // public float speed = 5;
    [Range(0,1f)] public float value = 0;

    void Update () {
        // value += Time.deltaTime * speed;
        for (float i = 0; i <= 1 - GameManager.singleton.curveInterval; i += GameManager.singleton.curveInterval) {
            Debug.DrawLine(GameManager.CubicInterp(PivotA, PivotB, i),GameManager.CubicInterp(PivotA, PivotB, i + GameManager.singleton.curveInterval));
        }
        for (float i = 0; i <= 1 - GameManager.singleton.curveInterval; i += GameManager.singleton.curveInterval) {
            Debug.DrawLine(GameManager.CubicInterp(PivotB, PivotC, i),GameManager.CubicInterp(PivotB, PivotC, i + GameManager.singleton.curveInterval));
        }


        Tester.position = GameManager.CubicInterp(PivotA, PivotB, value);
    }




}