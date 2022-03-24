using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField]private TextMeshProUGUI currencyCount;

    [SerializeField] private TextMeshProUGUI stageRecordText;

    [SerializeField] private TextMeshProUGUI scoreRecordText;

    public Sprite CurrentKnifeSprite { get; set; }

    private int appleCount;

    private int stageRecord;

    private int scoreRecord;

    private float time;
    // Start is called before the first frame update
    private void Awake()
    {
        //CurrentKnifeSprite = GameObject.Find("KnifeImage").GetComponent<Image>().sprite;
        
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
        appleCount = PlayerPrefs.GetInt("Apple", 0);
        stageRecord = PlayerPrefs.GetInt("Stage", 0);
        scoreRecord = PlayerPrefs.GetInt("Score", 0);
        SetAppleCount();
        SetRecordText();
    }

    private void SetAppleCount()
    {
        currencyCount.text = appleCount.ToString();
    }

    public void SetRecordText()
    {
        stageRecordText.text = "STAGE" + " " + stageRecord;
        scoreRecordText.text = "SCORE" + " " + scoreRecord;
    }

    public int GetScoreRecord()
    {
        return scoreRecord;
    }

    public void SetScoreRecord(int record)
    {
        if (record > scoreRecord)
        {
            scoreRecord = record;
        }
    }
    
    public void SetStageRecord(int record)
    {
        if (record > scoreRecord)
        {
            stageRecord = record;
        }
    }

    public int GetStageRecord()
    {
        return stageRecord;
    }

    void Start()
    {
        
    }

    private void OnDestroy()
    {
        PlayerPrefs.SetInt("Apple",appleCount);
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time >= 10.0f)
        {
            appleCount++;
            SetAppleCount();
            time = 0.0f;
        }
    }
}
