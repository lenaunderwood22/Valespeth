using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterMovement : MonoBehaviour {

    public Transform RootTran;
    public GameObject InputCanvas;

    public float speed = 2f;
    public float maxDistance = 10f;

    //private

    Vector3 currentPos;
    Vector3 startingPosition;

    void Start(){
        startingPosition = RootTran.transform.position;
    }

    void Update() {
        float axisVal = Input.GetAxisRaw("Horizontal");

        currentPos.x += axisVal * speed * Time.deltaTime;
        currentPos.x = Mathf.Clamp(currentPos.x, -maxDistance, maxDistance);

        RootTran.transform.position = startingPosition + currentPos;
    }

    public void Move (float axisVal) {
        currentPos.x += axisVal * speed * Time.deltaTime;
        currentPos.x = Mathf.Clamp(currentPos.x, -maxDistance, maxDistance);

        RootTran.transform.position = startingPosition + currentPos;
    }
}
