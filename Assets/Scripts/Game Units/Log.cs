using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Log : MonoBehaviour
{
    #region Inspector
    
    [SerializeField] private RotationOption[] rotationOptions;
    
    [SerializeField] private Rigidbody2D connectedRb;
    
    [SerializeField] private SpriteRenderer flashedWheel;

    [SerializeField] private GameObject destroyedWheel;

    [SerializeField] private GameObject destroyedWheelPrefab;

    [SerializeField] private List<Rigidbody2D> destroyedWheelPieces;
    
    public UnityEvent onRotationDestroy;
    public bool IsWaitNewStage { get; private set; }
    
    private int rotationOptionIndex;
    
    private float currentTime;
    //private float lastCurveTime; 
    #endregion

    #region MonoBehaviour
    
    private void Awake()
    {
        onRotationDestroy.AddListener(DestroyWheel);
        onRotationDestroy.AddListener(GenerateRotationOption);
        IsWaitNewStage = false;
    }
    
    void Start()
    {
        GenerateRotationOption();
    }
    
    void Update()
    {
        currentTime += Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Knife"))
        {
            StopCoroutine(Flashing());
            StartCoroutine(Flashing());
        }
    }

    private void FixedUpdate()
    {
        connectedRb.angularVelocity = rotationOptions[rotationOptionIndex].Speed*
                                      rotationOptions[rotationOptionIndex].MoveCurve.Evaluate(currentTime);
    }
    
    #endregion
    
    public void GenerateRotationOption()
    {
        rotationOptionIndex = Random.Range(0, rotationOptions.Length);
        /*lastCurveTime = rotationOptions[rotationOptionIndex].MoveCurve
            .keys[rotationOptions[rotationOptionIndex].MoveCurve.length].time;*/
    }
    
    private void DestroyWheel()
    {
        destroyedWheel.SetActive(true);
        destroyedWheel.transform.parent = null;
        Vibration.Vibrate();
        for (int i = 0; i < destroyedWheelPieces.Count; i++)
        {
            destroyedWheelPieces[i].transform.parent = null;
            Vector3 forceDir = (destroyedWheelPieces[i].transform.position - transform.position).normalized * 4;
            destroyedWheelPieces[i].AddForceAtPosition(forceDir,transform.position,ForceMode2D.Impulse);
            destroyedWheelPieces[i].AddTorque(4,ForceMode2D.Impulse);
            Destroy(destroyedWheelPieces[i].gameObject,2.0f);
        }
        Destroy(destroyedWheel);
        destroyedWheel = Instantiate(destroyedWheelPrefab, transform.position, Quaternion.identity, transform.GetChild(0));
        for (int i = 0; i < destroyedWheel.transform.childCount; i++)
        {
            destroyedWheelPieces[i] = destroyedWheel.transform.GetChild(i).GetComponent<Rigidbody2D>();
        }
        destroyedWheel.SetActive(false);
        IsWaitNewStage = true;
        transform.position = new Vector3(-5, 2, 0);
        StartCoroutine(WaitNewStage());
    }
    
    #region Coroutines

    private IEnumerator Flashing()
    {
        float startTime = Time.time;
        float duration = 0.025f;
        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            flashedWheel.color = new Color(flashedWheel.color.r, flashedWheel.color.g, flashedWheel.color.b,
                Mathf.Lerp(0, 1, t));
            yield return null;
        }

        startTime = Time.time;
        duration = 0.15f;

        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            t = 1 - Mathf.Abs(Mathf.Pow(t - 1, 2));
            flashedWheel.color = new Color(flashedWheel.color.r, flashedWheel.color.g, flashedWheel.color.b,
                Mathf.Lerp(1, 0, t));
            yield return null;
        }
        flashedWheel.color = new Color(flashedWheel.color.r, flashedWheel.color.g, flashedWheel.color.b,0);
    }
    
    private IEnumerator WaitNewStage()
    {
        yield return new WaitForSeconds(1.5f);
        transform.position = new Vector3(0, 2, 0);
        IsWaitNewStage = false;
    }

    #endregion

    #region TRASH
    private void SpawnKnife()
    {
        /*
        if (activeKnife)
        {
            activeKnife.onKnifeHit.RemoveAllListeners();
        }
        activeKnife = Instantiate(knifePrefab, new Vector3(0f, -2.78f, 0f), Quaternion.identity).GetComponent<Knife>();
        if (GameManager.Instance)
        {
            activeKnife.GetComponentInChildren<SpriteRenderer>().sprite = GameManager.Instance.CurrentKnifeSprite;
        }
        SetEvents();
        */
    }
    private void SetEvents()
    {
        /*
        activeKnife.onKnifeHit.AddListener(() => AddKnife(activeKnife));
        activeKnife.onKnifeHit.AddListener(SpawnKnife);
        activeKnife.onKnifeHit.AddListener(()=>StartCoroutine(Flashing()));
        */
    }
    private void AddKnife(Knife knife)
    {
        /*
        knifesPanel.transform.GetChild(knifesInWheel.Count - startKnifes).GetComponent<Image>().color = Color.black;
        knifesInWheel.Add(knife);
        knifesCountText.text = (int.Parse(knifesCountText.text.ToString())+1).ToString();
        if (knifesInWheel.Count - startKnifes > knifesToWin - 1)
        {
            foreach (var _knife in knifesInWheel)
            {
                _knife.gameObject.transform.SetParent(null);
                _knife.GetComponent<BoxCollider2D>().isTrigger = true;
                var rb = _knife.GetComponent<Rigidbody2D>();
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
                rb.AddTorque(3,ForceMode2D.Impulse);
                Destroy(_knife.gameObject,2.0f);
            }
            for (int i = startKnifes; i < knifesToWin; i++)
            {
                knifesPanel.transform.GetChild(i).GetComponent<Image>().color = defaultKnifeIconColor;
            }
            Instantiate(knifeIconPrefab, knifesPanel.transform);
            knifesToWin++;
            stage++;
            knifesInWheel.Clear();
            DestroyWheel();
            destroyedWheel = Instantiate(destroyedWheelPrefab, transform.position, Quaternion.identity, transform.GetChild(0));
            for (int i = 0; i < destroyedWheel.transform.childCount; i++)
            {
                destroyedWheelPieces[i] = destroyedWheel.transform.GetChild(i).GetComponent<Rigidbody2D>();
            }
            destroyedWheel.SetActive(false);
            transform.position = new Vector3(-5, 2, 0);
            GenerateLevel();
            StartCoroutine(WaitNewStage());
        }
        */
    }
    public void OnLoseGame()
    {
        /*
        stageCountText.text = stage.ToString();
        gameObject.SetActive(false);
        if (stage > GameManager.Instance.GetStageRecord())
        {
            GameManager.Instance.SetStageRecord(stage);
        }

        if (int.Parse(knifesCountText.text) > GameManager.Instance.GetScoreRecord())
        {
            GameManager.Instance.SetScoreRecord(int.Parse(knifesCountText.text));
        }
        
        GameManager.Instance.SetRecordText();

        for (int i = 0; i < objToRestart.Count; i++)
        {
            objToRestart[i].SetActive(true);
            //Destroy(gameObject);
        }*/
        
    }
    private void GenerateLevel()
    {
        /*
        rotationOptionIndex = Random.Range(0, rotationOptions.Length);
        knifesToWin = Random.Range(5, 14);
        var startknifes = Random.Range(1, 4);
        startKnifes = startknifes;
        float maxAngle = 360 / (float) startknifes;
        float lastAngle = 0;
        for (int i = 0; i < startknifes; i++)
        {
            float angle = lastAngle + Random.Range(20, maxAngle) * Mathf.Deg2Rad;
            lastAngle = angle;
            Vector3 pos = transform.position + new Vector3(Mathf.Sin(angle), Mathf.Cos(angle), 0) * WheelRadius;
            var knife = Instantiate(knifePrefab,pos,Quaternion.identity);
            knifesInWheel.Add(knife.GetComponent<Knife>());
            knife.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            knife.transform.up = (transform.position - knife.transform.position).normalized;
            knife.transform.parent = transform.GetChild(0);
            knife.GetComponent<BoxCollider2D>().offset = new Vector2(knife.GetComponent<BoxCollider2D>().offset.x, -0.69f);
            knife.GetComponent<BoxCollider2D>().size = new Vector2(knife.GetComponent<BoxCollider2D>().size.x, 1.15f);
        }
        */
    }
    #endregion
    
    
    
   

    
    
    

    
}