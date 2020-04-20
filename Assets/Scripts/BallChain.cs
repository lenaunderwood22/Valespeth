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

    public int BallSpawnAmount = 20;

    int spawnedBallsAmount = 0;

    float ballAppearanceTime; // Frequency of balls appearing in chain
    float timer;

    bool isFinished = false;

    GameManager GM;

    void Start() {
        GM = GameManager.singleton;

        timer = -GM.TimerDelay;

        ballAppearanceTime = GM.BallDiameter / GM.RollingSpeed;
    }

    void FixedUpdate() {
        if (isFinished) {
            return;
        }

        if (spawnedBallsAmount < BallSpawnAmount) {
            timer += Time.fixedDeltaTime;
        } else {
            timer = 0;
        }


        if(timer > ballAppearanceTime && spawnedBallsAmount < BallSpawnAmount) {
            timer = 0;

            AddRoller();
        }

        if (!GM.GameIsOver) {
            if (rollers.Count > 0) {
                if (Vector3.Distance(Curve.path.localPoints[Curve.path.localPoints.Length - 1], rollers[0].transform.position) < GM.BallDiameter) {
                    GM.LoseTheGame();
                }
            } else {
                if (spawnedBallsAmount == BallSpawnAmount) {
                    GM.AppendBallChainsCount();

                    isFinished = true;
                }
            }
        }

        UpdateBalls(Time.fixedDeltaTime * GM.RollingSpeed);
    }

    void UpdateBalls (float distance) {
        for(int i = ballsPosition.Count - 1; i >= 0; i--) {
            if(i < ballsPosition.Count - 1 && (ballsPosition[i] - ballsPosition[i + 1]) > GM.BallDiameter) {
                break;  // Don't move any more balls until the last part has caught up
            }
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

        spawnedBallsAmount++;
    }

    public void AddRoller (int atIndex, Color ballColor) {
        GameObject ball = CreateNewBall();
        ball.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", ballColor);

        float futurePosition;
        
        if (spawnedBallsAmount < BallSpawnAmount) {
            futurePosition = ballsPosition[atIndex];
        } else {
            futurePosition = ballsPosition[(atIndex-1) < 0 ? 0 : (atIndex-1)];
        }


        float offset = (ballAppearanceTime - timer) * GM.RollingSpeed;
        timer = 0;

        SubtractTill(atIndex-1, GM.BallDiameter);

        ballsPosition.Insert(atIndex, futurePosition);
        rollers.Insert(atIndex, ball);
        ballsColors.Insert(atIndex, ballColor);

        UpdateBalls(offset);
    }

    GameObject CreateNewBall() {
        GameObject newBall = Instantiate(GM.RollerPrefab);

        newBall.transform.localScale = Vector3.one * GM.BallDiameter;

        newBall.transform.SetParent(transform);

        return newBall;
    }
}
