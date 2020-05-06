using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour { 

    public static GameManager singleton = null;

    void Awake () {
        if (singleton == null) {
            singleton = this;
        } else {
            Destroy(this);
        }

        Time.timeScale = 1;
    }
    
    //[ColorUsageAttribute(true,true)] 
    public List<Color> BallColors;
    public float ColorEmissionIntensity;

    [HideInInspector] public bool GameIsFinished = false;

    public float BallDiameter = 1f;
    public float FinishPointDistanceOffset = 0.5f;
    public float ShootRate = 0.3f;

    public LayerMask RollerLayer;

    public Shooter MainShooter;
    public ShooterMovement MainShooterMovement;

    [Header("Win/Lose References")]
    public GameObject WinPanel;
    public GameObject LosePanel;
    public EventSystem m_EventSystem;
    public GraphicRaycaster m_Raycaster;

    private float shootTimer = 0;

    PointerEventData m_PointerEventData;


    void Update () {
        //#if UNITY_STANDALONE
        if (Input.GetKeyDown(KeyCode.Space) && shootTimer > ShootRate) {
            MainShooter.ShootProjectile();
            shootTimer = 0;
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            BackToTitleScreen();
        }
        //#endif

        #if UNITY_IOS || UNITY_ANDROID

        if (Input.touchCount > 0) {
            int tCount = Input.touchCount;

            for (int i = 0; i < tCount; i++) {
                Touch touch = Input.GetTouch(i);

                UIRaycast(touch.position);
            }
        }

        #endif

        shootTimer += Time.deltaTime;
    }

    public void BackToTitleScreen () {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void LoseGame () {
        Time.timeScale = 0;
        GameIsFinished = true;
        MainShooterMovement.InputCanvas.SetActive(false);

        Debug.Log("Lost the Game!");
        LosePanel.SetActive(true);
    }

    public void WinGame () {
        Time.timeScale = 0;
        GameIsFinished = true;
        MainShooterMovement.InputCanvas.SetActive(false);

        Debug.Log("Won the Game!");
        WinPanel.SetActive(true);
    }

    void UIRaycast (Vector2 touchPosition) {
        m_PointerEventData = new PointerEventData(m_EventSystem);
        m_PointerEventData.position = touchPosition;

        List<RaycastResult> results = new List<RaycastResult>();

        m_Raycaster.Raycast(m_PointerEventData, results);

        if (results.Count != 0) {
            var btn = results[0].gameObject.GetComponent<InputButton>();

            if (btn != null) {
                switch (btn.Type) {
                    case ButtonType.Shoot:
                        if (shootTimer > ShootRate) {
                            MainShooter.ShootProjectile();
                            shootTimer = 0;
                        }

                        break;
                    
                    case ButtonType.Left:
                        MainShooterMovement.Move(-1);
                        break;

                    case ButtonType.Right:
                        MainShooterMovement.Move(1);
                        break;
                    default:
                        break;
                }
            }
        }
    }

}