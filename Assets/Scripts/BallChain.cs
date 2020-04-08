using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class BallChain : MonoBehaviour{

    [Header("References")]
    [SerializeField] PathCreator Curve;

    // [Header("Properties")]
    

    //private
    
    List<GameObject> rollers = new List<GameObject>();
    List<Color> ballsColors = new List<Color>();
    List<float> ballsPosition = new List<float>();

    float rateByTime;
    float timer;

    GameManager GM;

    void Start() {
        GM = GameManager.singleton;

        timer = -GM.TimerDelay;

        rateByTime = GM.RateByDistance / GM.RollingSpeed;
    }

    void FixedUpdate() {
        timer += Time.fixedDeltaTime;

        if(timer > rateByTime) {
            timer = 0;

            AddRoller();
        }

        UpdateBalls(Time.fixedDeltaTime * GM.RollingSpeed);
    }

    void UpdateBalls (float distance) {
        for(int i = 0; i < ballsPosition.Count; i++) {
            ballsPosition[i] += distance;
            rollers[i].transform.position = Curve.path.GetPointAtDistance(ballsPosition[i]);
        }
    }

    void SubtractTill (int tillIndex, float amount) {
        for(int i = rollers.Count-1; i > tillIndex; i--) {
            ballsPosition[i] -= amount;
            rollers[i].transform.position = Curve.path.GetPointAtDistance(ballsPosition[i]);
        }
    }

    public void CheckChainAt (int atIndex) {
        Color sourceCol = ballsColors[atIndex];

        int leftMost = ChainLength(atIndex, sourceCol, 1);
        int rightMost = ChainLength(atIndex, sourceCol, -1);

        if (leftMost - rightMost + 1 > 2) {
            for (int i = leftMost; i >= rightMost; i--) {
                Destroy(rollers[i]);
                
                rollers.RemoveAt(i);
                ballsPosition.RemoveAt(i);
                ballsColors.RemoveAt(i);
            }
        }
    }

    int ChainLength (int index, Color source, int increment) {
        if (ballsColors[index] == source) {
            if (index + increment < 0 || index + increment > rollers.Count - 1) {
                return index;
            }

            return ChainLength(index + increment, source, increment);
        } else {
            return index - increment;
        }
    }

    public int GetIndexOfRoller (GameObject rollerObj) {
        return rollers.IndexOf(rollerObj);
    }

    public void AddRoller () {
        GameObject ball = CreateNewBall();
        Color col = GM.BallColors[Random.Range(0, GM.BallColors.Count)];

        ball.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", col);
        
        ballsPosition.Add(0);
        rollers.Add(ball);
        ballsColors.Add(col);
    }

    public void AddRoller (int atIndex, Color ballColor) {
        GameObject ball = CreateNewBall();
        ball.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", ballColor);

        float futurePosition = ballsPosition[atIndex];

        float offset = (rateByTime - timer) * GM.RollingSpeed;
        timer = 0;

        SubtractTill(atIndex-1, GM.RateByDistance);

        ballsPosition.Insert(atIndex, futurePosition);
        rollers.Insert(atIndex, ball);
        ballsColors.Insert(atIndex, ballColor);

        UpdateBalls(offset);
    }

    GameObject CreateNewBall() {
        GameObject newBall = Instantiate(GM.RollerPrefab);

        newBall.transform.SetParent(transform);

        return newBall;
    }
}
