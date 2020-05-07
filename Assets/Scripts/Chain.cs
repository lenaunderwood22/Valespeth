using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PathCreation;
using TMPro;

public class Chain : MonoBehaviour {

    public PathCreator Curve;
    public Roller RollerPrefab;
    public GameObject collisionText;

    public int RollerCount = 10;
    public float MoveSpeed = 1f;

    public float ColorCheckRate = 0.2f;

     public Material newMat;

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

     //Explosion
    float sphereSize = 0.5f;
    int spheresInRow = 3;

    float radius = 1.3f;

    float up = 1.5f;

    float force = 1.3f;

   

    Vector3 cubePivot;

    Vector3 explosionPos;

    List<GameObject> pieces = new List<GameObject>();

    //Text
    [HideInInspector] public int currentHitCount;
    int previousHitCount = 0;


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

        Vector3 TextPos = Rollers[till].GetComponent<Transform>().position;
        collisionText.GetComponent<TextMeshPro>().color = gm.BallColors[Rollers[till].RollerColorID] + new Color32(0, 0, 0, 255);
        if(previousHitCount != 0 && currentHitCount - previousHitCount == 1){
                ShowText(TextPos, "Streak!");
            }else if(previousHitCount != 0 && previousHitCount == currentHitCount){
                ShowText(TextPos, "Streak!");
            }
            else if((till - from) == 3){
                ShowText(TextPos, "Nice!");
            }else if((till - from) == 4 ){
                ShowText(TextPos, "Great!");
            }else if(till - from == 5 ){
                ShowText(TextPos, "Amazing!");
            }else if(till - from > 5 ){
                ShowText(TextPos, "Wonderful!");
            }else{
                ShowText(TextPos, "Hit!");
            }

        for (int i = till; i >= from; i--) {
            int PiecesColor = Rollers[i].RollerColorID;
            Vector3 explosionPos = Rollers[i].GetComponent<Transform>().position;
            explode(explosionPos, PiecesColor);

            Destroy(Rollers[i].gameObject);
        }

        previousHitCount = currentHitCount; 
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

    #region EffectsForHitExplosion

    private void creatPieces(int x, int y, int z, Vector3 explosionPos, int PiecesColorID){
        GameObject piece;
        piece = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        piece.transform.position = explosionPos + new Vector3(x*sphereSize, y*sphereSize, z*sphereSize) - cubePivot;
        piece.transform.localScale = new Vector3(sphereSize, sphereSize, sphereSize);

        
        piece.GetComponent<MeshRenderer>().material = newMat;
        piece.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", gm.BallColors[PiecesColorID] * gm.ColorEmissionIntensity);
    
        // piece.GetComponent<SphereCollider>().isTrigger = true;

        piece.AddComponent<Rigidbody>();
        piece.GetComponent<Rigidbody>().mass = sphereSize;
        
        pieces.Add(piece);
        
        Destroy(piece, 1.5f);
        
    }

    private void explode(Vector3 explosionPos, int PiecesColorID){

        // gameObject.SetActive(false);

    

        for(int x = 0; x < spheresInRow; x++){
            for(int y = 0; y < spheresInRow; y++){
                for(int z = 0; z < spheresInRow; z++){
                    creatPieces(x,y,z,explosionPos, PiecesColorID);
                }
            }
        }

    
        // Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
        foreach(GameObject nearby in pieces){
            Rigidbody rb = nearby.GetComponent<Rigidbody>();
            nearby.SetActive(true);
            if(rb != null){
                rb.AddExplosionForce(force, explosionPos, radius, up, ForceMode.Impulse);
                rb.AddForce(new Vector3(0f, 0f, 3f), ForceMode.Impulse);

            }
        }

        pieces.Clear();
    }

    #endregion

    #region EffectsForHitText

    private void ShowText(Vector3 TextLocation, string words){
        collisionText.transform.position = TextLocation + new Vector3(10f, 0f, 0f);
        collisionText.GetComponent<TextMeshPro>().text = words;
        collisionText.SetActive(true); 
        Invoke("DeactivateText", 2);  
        
    }

    private void DeactivateText()
     {
         collisionText.SetActive(false);      
 
     }

    #endregion
}