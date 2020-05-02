using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterMovement : MonoBehaviour {

    public Transform RootTran;

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


    //This function is used for mobile touches
    //Function called in GameManager.cs Update()
    public void Move (float x)
    {
        currentPos.x = x;
        RootTran.transform.position = startingPosition + currentPos;
    }
}
