using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class Roller : MonoBehaviour {

    public int RollerColorID;
    public Transform Tran;
    public Transform GraphicsRootTran;
    public MeshRenderer GraphicsMeshRend;

    public float InsertSpeed = 0.125f;

    [HideInInspector] public Chain chain;
    public bool IsAnimating { get { return isAnimating; } }
    bool isAnimating = false;

    public float GetCurrentPosition { get { return pos; } }

    PathCreator curve;

    float pos;
    float maxPos;

    // float speed = 0;
    float speedDelta = 0;

    float rotationSpeed = 45;


    Vector3 currentEulerAngles;
    Quaternion currentRotation;

    GameManager gm;

    public void ActivateRoller (Chain _chain, float startPos, int rollerColorID) {
        chain = _chain;
        curve = _chain.Curve;

        // speed = _chain.MoveSpeed;

        gm = GameManager.singleton;

        pos = startPos;

        RollerColorID = rollerColorID;
        maxPos = chain.Curve.path.length - gm.FinishPointDistanceOffset;

        GraphicsMeshRend.material.SetColor("_BaseColor", gm.BallColors[RollerColorID] * gm.ColorEmissionIntensity);
    }

    public void PlayInsertAnimation (Vector3 animStartPos) {
        StartCoroutine(InsertAnimation(animStartPos));
    }

    public void UpdatePositionBySpeed (float atSpeed) {
        pos += atSpeed * Time.deltaTime;
        transform.position = curve.path.GetPointAtDistance(pos);

        currentEulerAngles += new Vector3(0.0f, 0.0f, -1.0f)  * Time.deltaTime * rotationSpeed;
        currentRotation.eulerAngles = currentEulerAngles;
        transform.rotation = currentRotation;
    }

    public void UpdatePositionByDelta (float positionDelta) {
        pos += positionDelta;
        transform.position = curve.path.GetPointAtDistance(pos);

        currentEulerAngles += new Vector3(0.0f, 0.0f, -1.0f)  * Time.deltaTime * rotationSpeed;
        currentRotation.eulerAngles = currentEulerAngles;
        transform.rotation = currentRotation;
    }

    void OnEnable () {
        Tran = transform;
    }

    void OnDestroy () {
        if (chain != null) {
            chain.RemoveRollerOnDestroy(this);
        } else {
            Debug.Log("WARNING: this should not happen if roller reference is not in the scene");
        }

    }

    void Update () {
        if (chain == null || gm.GameIsFinished) {
            return;
        }

        if (pos >= maxPos) {
            GameManager.singleton.LoseGame();
        }

        // pos += (speed + speedDelta) * Time.deltaTime;
        // transform.position = curve.path.GetPointAtDistance(pos);
    }

    IEnumerator InsertAnimation (Vector3 animStartPos) {
        isAnimating = true;
        GraphicsRootTran.position = animStartPos;

        while (Vector3.Distance(GraphicsRootTran.position, Tran.position) > 0.05f) {
            GraphicsRootTran.position = Vector3.Lerp(GraphicsRootTran.position, Tran.position, InsertSpeed);

            yield return null;
        }

        GraphicsRootTran.localPosition = Vector3.zero;
        isAnimating = false;
    }

}