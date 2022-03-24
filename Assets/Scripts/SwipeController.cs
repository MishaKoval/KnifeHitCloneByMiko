using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwipeController : MonoBehaviour
{
    // Start is called before the first frame update

    private RectTransform _rectTransform;

    [SerializeField] private CanvasScaler scaler;

    private int currentNumber;

    private int maxNumber;

    [SerializeField] private GameObject slider;

    [SerializeField] private Image[] slidersRenderers;

    private List<Button> knifesButtons;

    private Color selectedColor;


    private void Awake()
    {
        knifesButtons = new List<Button>();
        //Debug.Log(transform.childCount);
        for (int i = 0; i < transform.childCount; i++)
        {
            var page = transform.GetChild(i);
            for (int j = 0; j < page.childCount; j++)
            {
                Button kButton;
                page.GetChild(j).TryGetComponent(out kButton);
                knifesButtons.Add(kButton);
            }    
        }

        foreach (var button in knifesButtons)
        {
            //button.onClick.AddListener(()=> Debug.Log(knifesButtons.IndexOf(button)));
            //button.onClick.AddListener();
            button.onClick.AddListener(()=>GameManager.Instance.CurrentKnifeSprite = button.gameObject.transform.GetChild(0).GetComponent<Image>().sprite);
            //button.onClick.AddListener(()=>Debug.Log(button.gameObject.transform.GetChild(0).GetComponent<Image>().sprite));
        }
    }

    void Start()
    {
        slidersRenderers = new Image[transform.childCount];

        for (var i = 0; i < slidersRenderers.Length; i++)
        {
            slidersRenderers[i] = slider.transform.GetChild(i).GetComponent<Image>();
        }
        selectedColor = slidersRenderers[0].color;
        currentNumber = 0;
        maxNumber = transform.childCount - 1;
        _rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(scaler.scaleFactor);
        if (Input.touchCount > 0)
        {
            //Debug.Log("Touch");
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                var prevNumber = currentNumber;
                if (Input.GetTouch(0).deltaPosition.x > 0)
                {
                    
                    if (currentNumber > 0)
                    {
                        
                        //Debug.Log("Right");
                        _rectTransform.anchoredPosition =
                            new Vector2(_rectTransform.anchoredPosition.x + 800,
                                _rectTransform.anchoredPosition.y);
                        currentNumber--;
                    }
                }
                else
                {
                    if (currentNumber < maxNumber)
                    {
                        //Debug.Log("Left");
                        _rectTransform.anchoredPosition =
                            new Vector2(_rectTransform.anchoredPosition.x - 800,
                                _rectTransform.anchoredPosition.y);
                        currentNumber++;
                    }
                }
                slidersRenderers[prevNumber].color = Color.black;
                slidersRenderers[currentNumber].color = selectedColor;
            }
        }
    }
}