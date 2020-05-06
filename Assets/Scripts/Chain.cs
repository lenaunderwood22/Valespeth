using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class Chain : MonoBehaviour {

    public PathCreator Curve;
    public Roller RollerPrefab;

    public int RollerCount = 10;
    public float MoveSpeed = 1f;

    public float ColorCheckRate = 0.2f;

    [HideInInspector] public List<Roller> Rollers = new List<Roller>();

    [Header("Stall Animation Properties")]
    public float StallDuration = 1f;
    
    int rollersSpawned = 0;

    Vector3 spawnPoint;
    LayerMask rollerMask;
    GameManager gm;

    float spawnCheckRadius;
    float timer = 0;
    float spawnCooldown = 0;
    float currentSpeed;

    bool freezeTimer = false;

    Coroutine currentStallCor;
    Coroutine colorCheckerCor;

    void Start () {
        gm = GameManager.singleton;

        spawnCheckRadius = gm.BallDiameter / 2;

        spawnCooldown = gm.BallDiameter / MoveSpeed;
        currentSpeed = MoveSpeed;

        rollerMask = gm.RollerLayer;

        spawnPoint = Curve.path.GetPoint(0);

        colorCheckerCor = StartCoroutine(ColorChecker());
    }

    void Update () {
        if (!freezeTimer)
            timer += Time.deltaTime;

        if (rollersSpawned < RollerCount) {
            if (timer >= spawnCooldown) {
                SpawnRoller();

                timer = 0;
            }
        }

        UpdateRollerPositions();
    }

    public int GetIndexOfRoller (Roller ofRoller) {
        return Rollers.IndexOf(ofRoller);
    }

    IEnumerator ColorChecker () {
        while (!gm.GameIsFinished) {
            yield return new WaitForEndOfFrame();

            int startIndex = Rollers.Count - 1;

            for (int i = startIndex - 1; i >= -1; i--) {
                if (i == -1) {
                    if (startIndex - i > 2) {
                        CheckAnimationAndRemove(i + 1, startIndex);
                    }

                    break;
                }

                if (Rollers[startIndex].RollerColorID != Rollers[i].RollerColorID) {
                    if (startIndex - i > 2) {
                        CheckAnimationAndRemove(i + 1, startIndex);
                    }

                    startIndex = i;
                }
            }

            yield return new WaitForSeconds(ColorCheckRate);
        }
    }

    void CheckAnimationAndRemove (int from, int till) {
        for (int i = till; i >= from; i--) {
            if (i != from) {
                if (Vector3.Distance(Rollers[i].Tran.position, Rollers[i-1].Tran.position) > gm.BallDiameter + 0.08f) {
                    //Debug.Log(Vector3.Distance(Rollers[i].Tran.position, Rollers[i-1].Tran.position));
                    return;
                }
            }

            if (Rollers[i].IsAnimating) {
                return;
            }
        }

        for (int i = till; i >= from; i--) {
            Destroy(Rollers[i].gameObject);
        }
    }

    #region Roller Adding
    Roller GenerateRoller (int newRollerColorID, float startPosition = 0) {
        Roller newRoller = Instantiate(RollerPrefab, spawnPoint, Quaternion.identity, transform);

        newRoller.ActivateRoller(this, startPosition, newRollerColorID);

        return newRoller;
    }

    void SpawnRoller () {
        int colorID = Random.Range(0, gm.BallColors.Count);

        while (CheckLastTwoRollers(colorID)) {
            colorID = Random.Range(0, gm.BallColors.Count);
        }

        Roller newRoller = GenerateRoller(colorID, 0);

        Rollers.Add(newRoller);
        rollersSpawned++;
    }

    bool CheckLastTwoRollers (int colorID) {
        int count = Rollers.Count;
        
        if (count < 2) {
            return false;
        }

        if (colorID == Rollers[count-1].RollerColorID && colorID == Rollers[count-2].RollerColorID) {
            return true;
        }

        return false;
    }

    public void InsertRollerAt (int atIndex, int _rollerColorID, Vector3 hitPoint) {
        float targetPosition;

        if (atIndex == 0) {
            targetPosition = gm.BallDiameter + Rollers[atIndex].GetCurrentPosition;
        } else {
            targetPosition = Rollers[atIndex-1].GetCurrentPosition;

            ChainMoveRollersByDelta(atIndex-1, gm.BallDiameter);
        }

        Roller newRoller = GenerateRoller(_rollerColorID, targetPosition);

        if (atIndex == Rollers.Count) {
            Rollers.Add(newRoller);
        } else if (atIndex > Rollers.Count) {
            Debug.LogError("Inserting at improper location");
        } else {
            Rollers.Insert(atIndex, newRoller);
        }

        newRoller.Tran.position = Curve.path.GetPointAtDistance(targetPosition);
        newRoller.PlayInsertAnimation(hitPoint);
    }

    #endregion

    #region Roller Position Update

    void UpdateRollerPositions () {
        if (Rollers.Count != 0) {
            ChainMoveRollersBySpeed(Rollers.Count-1, currentSpeed);
        }
    }

    void ChainMoveRollersBySpeed (int index, float atSpeed) {
        Rollers[index].UpdatePositionBySpeed(atSpeed);

        if (index == 0) {
            return;
        } else if (Vector3.Distance(Rollers[index].Tran.position, Rollers[index-1].Tran.position) > gm.BallDiameter) {
            return;
        }

        ChainMoveRollersBySpeed(index-1, atSpeed);
    }

    void ChainMoveRollersByDelta (int index, float delta) {
        Rollers[index].UpdatePositionByDelta(delta);

        if (index == 0) {
            return;
        } else if (Vector3.Distance(Rollers[index].Tran.position, Rollers[index-1].Tran.position) > gm.BallDiameter) {
            Debug.Log("Returned at " + index);

            return;
        }

        ChainMoveRollersByDelta(index-1, delta);
    }
    #endregion

    #region Roller Remove

    public void RemoveRollerOnDestroy (Roller rollerToRemove) {
        try {
            RemoveRoller(rollerToRemove);
        } catch {
            Debug.Log("Roller does not exist in the list");
        }
    }

    void RemoveRoller (int _atIndex) {
        Rollers.RemoveAt(_atIndex);

        OnRollerRemoved();
    }

    void RemoveRoller (Roller _roller) {
        Rollers.Remove(_roller);

        OnRollerRemoved();
    }

    void OnRollerRemoved () {
        if (!gameObject.activeSelf)
            return;

        if (currentStallCor != null) {
            StopCoroutine(currentStallCor);
        }

        if (Rollers.Count == 0) {
            GameManager.singleton.WinGame();

            return;
        }

        currentStallCor = StartCoroutine(StallAnim());
    }

    #endregion

    #region Animation

    IEnumerator StallAnim () {
        float timer = 0;
        freezeTimer = true;

        while (timer < StallDuration) {
            currentSpeed = 0;

            timer += Time.deltaTime;
            yield return null;
        }

        freezeTimer = false;

        currentSpeed = MoveSpeed;
    }

    #endregion
}