using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class ballManager : MonoBehaviour
{
    public static ballManager singleton = null;

    // [Range(0f,1f)]public float GameTimeScale = 1f;

    public ballShooter BallShooter;
    public List<GameObject> balls = new List<GameObject>();
    public List<float> ballsPosition = new List<float>();
    public float timer = -2;
    public float rateByDistance = 1f;
    public float speed = 1.5f;
    public PathCreator pathCreator;

    public List<Color> ColorList;

    public GameObject ballPrefab;

    private float rateByTime;

    void Awake () {
        if (singleton == null) {
            singleton = this;
        } else {
            Debug.LogError("Es ar unda momxdariyo. 2 ball manager gaq scenashi da erti washale");
            Destroy(this);
        }
    }

    void Start()
    {
        rateByTime = rateByDistance / speed;
    }

    void Update()
    {
        //!TEMP
        // Time.timeScale = GameTimeScale;

        timer += Time.deltaTime;

        if(Input.GetKeyDown(KeyCode.Space)) {
            BallShooter.shoot();
        }

        if(timer > rateByTime) {
            timer = 0;

            AddBall();
        }

        UpdateBalls(Time.deltaTime * speed);
    }

    void UpdateBalls (float distance) {
        for(int i = 0; i < ballsPosition.Count; i++) {
            ballsPosition[i] += distance;
            balls[i].transform.position = pathCreator.path.GetPointAtDistance(ballsPosition[i]);
        }
    }

    void SubtractTill (int tillIndex, float amount) {
        for(int i = balls.Count-1; i > tillIndex; i--) {
            ballsPosition[i] -= amount;
            balls[i].transform.position = pathCreator.path.GetPointAtDistance(ballsPosition[i]);
        }
    }

    public void AddBall () {
        GameObject ball = Instantiate(ballPrefab);
        ball.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", ColorList[Random.Range(0, ColorList.Count)]);
        
        ballsPosition.Add(0);
        balls.Add(ball);
    }

    public void AddBall (int atIndex, Color ballColor) {
        GameObject ball = Instantiate(ballPrefab);
        ball.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", ballColor);

        float futurePosition = ballsPosition[atIndex];

        float offset = (rateByTime - timer) * speed;
        timer = 0;

        SubtractTill(atIndex-1, rateByDistance);

        ballsPosition.Insert(atIndex, futurePosition);
        balls.Insert(atIndex, ball);

        UpdateBalls(offset);
    }
}
