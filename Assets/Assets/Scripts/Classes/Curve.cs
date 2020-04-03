using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Curve {

    public Transform[] Pivots;

    public float TotalLength = -1;

    List<float> lengths = new List<float>();

    public void CalculateAllLengths () {
        lengths.Clear();
        TotalLength = 0;

        for (int i = 1; i < Pivots.Length; i++) {
            float currentLength = CalculateLength(i - 1, i);

            lengths.Add(currentLength);

            TotalLength += currentLength;
        }

        Debug.Log(TotalLength);
    }

    public float CalculateLength (int start, int end) {
        float length = 0;

        for (float i = 0; i <= 1 - GameManager.singleton.curveInterval; i += GameManager.singleton.curveInterval) {
            length += Vector3.Distance(GameManager.CubicInterp(Pivots[start], Pivots[end], i), GameManager.CubicInterp(Pivots[start], Pivots[end], i + GameManager.singleton.curveInterval));
        }

        return length;
    }
}