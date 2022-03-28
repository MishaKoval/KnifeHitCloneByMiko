using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    private enum GameState
    {
        Menu = 0,
        Game = 1
    }

    private const float WheelRadius = 1.084377f;

    public static GameManager Instance { get; private set; }

    #region UIElements

    [Header("UIELEMENTS")] [SerializeField]
    private TextMeshProUGUI currencyCount;

    [SerializeField] private TextMeshProUGUI stageRecordText;

    [SerializeField] private TextMeshProUGUI scoreRecordText;

    [SerializeField] private TextMeshProUGUI knifesCountText;

    [SerializeField] private TextMeshProUGUI stageCountText;

    [SerializeField] private TextMeshProUGUI hitKnifesCounter;

    #endregion

    #region SerializeFields

    [Header("Prefabs")] [SerializeField] private GameObject knifePrefab;

    [SerializeField] private GameObject applePrefab;

    [SerializeField] private GameObject knifeIconPrefab;

    [Header("Other fields")] [SerializeField]
    private GameObject knifesPanel;

    [SerializeField] private Color defaultKnifeIconColor;

    [SerializeField] private GameObject restartButton;

    [SerializeField] private GameObject homeButton;

    [SerializeField] private GameObject afterLoseElements;

    [SerializeField] private Log log;

    [SerializeField] private ShopController shopController;

    [SerializeField] private Image menuKnifeImage;

    [SerializeField] private AppleSpawner appleSpawner;

    #endregion

    public Sprite CurrentKnifeSprite { get; set; }

    #region PrivateField

    private Knife activeKnife;

    private GameObject appleOnLog;

    private List<Knife> knifeInLog;

    private GameState gameState;

    private int appleCount;

    private int stageRecord;

    private int scoreRecord;

    private int knifesToWin;

    //private int startKnifes;

    private int knifesInLog;

    private int stage;

    private float time;

    //private int selectedKnifeId;

    #endregion

    #region MonoBehaviour

    private void Awake()
    {
        CurrentKnifeSprite = menuKnifeImage.sprite;
        appleOnLog = null;
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else
        {
            Destroy(gameObject);
        }

        Vibration.Init();
        time = 0.0f;
        gameState = GameState.Menu;
        appleCount = PlayerPrefs.GetInt("Apple", 0);
        stageRecord = PlayerPrefs.GetInt("Stage", 0);
        scoreRecord = PlayerPrefs.GetInt("Score", 0);
        knifesToWin = 6;
        knifesInLog = 0;
        knifeInLog = new List<Knife>();
        activeKnife = null;
        stage = 1;
        for (int i = 0; i < knifesToWin; i++)
        {
            Instantiate(knifeIconPrefab, knifesPanel.transform);
        }

        defaultKnifeIconColor = knifeIconPrefab.GetComponent<Image>().color;
        SetAppleCount();
        SetRecordText();
        Application.targetFrameRate = 60;
    }

    private void Update()
    {
        if (gameState == GameState.Game)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (activeKnife && !log.IsWaitNewStage)
                {
                    if (!activeKnife.GetComponent<Knife>().isHited)
                    {
                        activeKnife.GetComponent<Rigidbody2D>()
                            .AddForce(activeKnife.transform.up.normalized * 30f, ForceMode2D.Impulse);
                        activeKnife.GetComponent<Rigidbody2D>().gravityScale = 1;
                        activeKnife.GetComponent<Knife>().isHited = true;
                    }
                }
            }
        }

        time += Time.deltaTime;
        if (time >= 10.0f)
        {
            appleCount++;
            SetAppleCount();
            time = 0.0f;
        }
    }


    private void OnDestroy()
    {
        SaveData();
    }

    public void ChangeKnifeInMenu()
    {
        menuKnifeImage.sprite = CurrentKnifeSprite;
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        SaveData();
    }

    private void OnApplicationQuit()
    {
        SaveData();
    }

    #endregion

    #region GameLogic

    public void GenerateGame()
    {
        ResetGame();
        gameState = GameState.Game;
        SpawnKnife();
    }

    private void SpawnKnife()
    {
        if (activeKnife)
        {
            activeKnife.onKnifeHit.RemoveAllListeners();
        }

        activeKnife = Instantiate(knifePrefab, new Vector3(0f, -2.78f, 0f), Quaternion.identity).GetComponent<Knife>();
        if (Instance)
        {
            activeKnife.GetComponentInChildren<SpriteRenderer>().sprite = Instance.CurrentKnifeSprite;
        }

        SetEvents();
    }

    private void ResetGame()
    {
        hitKnifesCounter.text = "0";
        knifesToWin = 6;
        knifesInLog = 0;
        knifeInLog = new List<Knife>();
        activeKnife = null;
        stage = 1;
        foreach (Transform child in knifesPanel.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < knifesToWin; i++)
        {
            Instantiate(knifeIconPrefab, knifesPanel.transform);
        }
    }

    private void GenerateLevel()
    {
        knifesToWin = Random.Range(5, 11);
        var startknifes = Random.Range(1, 4);
        float maxAngle = 360 / (float) startknifes;
        float lastAngle = 0;
        for (int i = 0; i < startknifes; i++)
        {
            float angle = lastAngle + Random.Range(20, maxAngle) * Mathf.Deg2Rad;
            lastAngle = angle;
            Vector3 pos = log.transform.position + new Vector3(Mathf.Sin(angle), Mathf.Cos(angle), 0) * WheelRadius;
            var knife = Instantiate(knifePrefab, pos, Quaternion.identity);
            knifeInLog.Add(knife.GetComponent<Knife>());
            knife.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            knife.transform.up = (log.transform.position - knife.transform.position).normalized;
            knife.transform.parent = log.transform;
            knife.GetComponent<BoxCollider2D>().offset =
                new Vector2(knife.GetComponent<BoxCollider2D>().offset.x, -0.69f);
            knife.GetComponent<BoxCollider2D>().size = new Vector2(knife.GetComponent<BoxCollider2D>().size.x, 1.15f);
        }

        if (Random.Range(0, 100) < appleSpawner.GetChance())
        {
            float angle = lastAngle + Random.Range(20, maxAngle) * Mathf.Deg2Rad;
            Vector3 pos = log.transform.position + new Vector3(Mathf.Sin(angle), Mathf.Cos(angle), 0) * WheelRadius;
            var apple = Instantiate(applePrefab, pos, Quaternion.identity);
            apple.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            apple.transform.up = (apple.transform.position - log.transform.position).normalized;
            apple.transform.parent = log.transform;
            appleOnLog = apple;
        }

        if (knifesToWin != knifesPanel.transform.childCount)
        {
            foreach (Transform child in knifesPanel.transform)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < knifesToWin; i++)
            {
                Instantiate(knifeIconPrefab, knifesPanel.transform);
            }
        }
    }

    public void LoseGame()
    {
        StartCoroutine(WaitAndLoseGame());
    }

    #endregion

    #region Records

    private void SetRecordText()
    {
        stageRecordText.text = "STAGE" + " " + stageRecord;
        scoreRecordText.text = "SCORE" + " " + scoreRecord;
    }

    private int GetScoreRecord()
    {
        return scoreRecord;
    }

    private void SetScoreRecord(int record)
    {
        if (record > scoreRecord)
        {
            scoreRecord = record;
        }
    }

    private void SetStageRecord(int record)
    {
        if (record > stageRecord)
        {
            stageRecord = record;
        }
    }

    private int GetStageRecord()
    {
        return stageRecord;
    }

    #endregion

    private void SaveData()
    {
        PlayerPrefs.SetInt("Apple", appleCount);
        PlayerPrefs.SetInt("Stage", stageRecord);
        PlayerPrefs.SetInt("Score", scoreRecord);
        if (shopController)
        {
            var unlocked = shopController.GetUnlockedButtons();
            if (unlocked != null)
            {
                foreach (var id in unlocked)
                {
                    PlayerPrefs.SetInt("Button" + id, 1);
                }
            }
        }
    }

    private IEnumerator WaitAndLoseGame()
    {
        yield return new WaitForSeconds(1.5f);
        foreach (var knife in knifeInLog)
        {
            Destroy(knife.gameObject);
        }

        if (appleOnLog) Destroy(appleOnLog);
        gameState = GameState.Menu;
        if (activeKnife) Destroy(activeKnife.gameObject);
        activeKnife = null;
        afterLoseElements.SetActive(true);
        homeButton.SetActive(true);
        knifesPanel.SetActive(false);
        knifesCountText.text = hitKnifesCounter.text;
        hitKnifesCounter.gameObject.SetActive(false);
        stageCountText.text = stage.ToString();
        stageCountText.transform.parent.gameObject.SetActive(true);
        restartButton.SetActive(true);
        log.GenerateRotationOption();
        log.transform.parent.gameObject.SetActive(false);
        if (stage > Instance.GetStageRecord())
        {
            Instance.SetStageRecord(stage);
        }

        if (int.Parse(knifesCountText.text) > Instance.GetScoreRecord())
        {
            Instance.SetScoreRecord(int.Parse(knifesCountText.text));
        }

        Instance.SetRecordText();
    }

    private void SetAppleCount()
    {
        currencyCount.text = appleCount.ToString();
    }

    public void AddApple()
    {
        appleCount += 2;
    }

    public bool UnlockKnife()
    {
        if (appleCount - 100 >= 0)
        {
            appleCount -= 100;
            SetAppleCount();
            return true;
        }

        ButtonsController.Instance.OnDontHaveApples();
        return false;
    }

    private void RefreshUI()
    {
        hitKnifesCounter.text = (Int32.Parse(hitKnifesCounter.text) + 1).ToString();
        knifesPanel.transform.GetChild(knifesInLog - 1).GetComponent<Image>().color = Color.black;
    }

    private void SetEvents()
    {
        activeKnife.onKnifeHit.AddListener(() => OnKnifeHit(activeKnife));
    }

    private void OnKnifeHit(Knife knife)
    {
        SpawnKnife();
        knifesInLog++;
        RefreshUI();
        knifeInLog.Add(knife);
        if (knifesInLog >= knifesToWin)
        {
            foreach (var hitedKnife in knifeInLog)
            {
                hitedKnife.gameObject.transform.SetParent(null);
                hitedKnife.GetComponent<BoxCollider2D>().isTrigger = true;
                hitedKnife.isAfterLogDestroy = true;
                var rb = hitedKnife.GetComponent<Rigidbody2D>();
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.gravityScale = 1.0f;
                switch (Random.Range(0, 4))
                {
                    case 0:
                        rb.AddForce(transform.up * 3f, ForceMode2D.Impulse);
                        break;
                    case 1:
                        rb.AddForce(transform.up * 6f, ForceMode2D.Impulse);
                        break;
                    case 2:
                        rb.AddForce(transform.up * 9f, ForceMode2D.Impulse);
                        break;
                    case 3:
                        rb.AddForce(transform.up * 12f, ForceMode2D.Impulse);
                        break;
                }

                rb.AddTorque(3, ForceMode2D.Impulse);
                Destroy(hitedKnife.gameObject, 2.0f);
            }

            for (int i = 0; i < knifesToWin; i++)
            {
                knifesPanel.transform.GetChild(i).GetComponent<Image>().color = defaultKnifeIconColor;
            }

            stage++;
            knifeInLog.Clear();
            knifesInLog = 0;
            log.onRotationDestroy?.Invoke();
            GenerateLevel();
        }
    }
}