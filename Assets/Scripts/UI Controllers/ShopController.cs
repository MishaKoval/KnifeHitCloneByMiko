using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
    #region Inspector

    [SerializeField] private CanvasScaler scaler;

    [SerializeField] private GameObject slider;

    [SerializeField] private Image[] slidersRenderers;

    [SerializeField] private Button unlockButton;

    [SerializeField] private float swipeDistance = 1.0f;

    //[SerializeField] private float swipeTime = 1.0f;

    #endregion

    #region PrivateFields

    private RectTransform rectTransform;

    private int currentNumber;

    private int maxNumber;

    private List<Button> knifesButtons;

    private ShopButton selectedButton;

    private Color selectedColor;

    private Coroutine swipeCoroutine;

    #endregion
    
    #region MonoBehaviour

    private void Awake()
    {
        unlockButton.onClick.AddListener(UnlockSelected);
        swipeCoroutine = null;
        knifesButtons = new List<Button>();
        for (int i = 0; i < transform.childCount; i++)
        {
            var page = transform.GetChild(i);
            for (int j = 0; j < page.childCount; j++)
            {
                Button kButton = null;
                page.GetChild(j).TryGetComponent(out kButton);
                if (kButton != null) knifesButtons.Add(kButton);
            }
        }

        knifesButtons[0].GetComponent<ShopButton>().UnlockItem();

        for (int i = 0; i < knifesButtons.Count; i++)
        {
            if (PlayerPrefs.HasKey("Button" + i))
            {
                knifesButtons[i].GetComponent<ShopButton>().UnlockItem();
            }
        }

        foreach (var button in knifesButtons)
        {
            if (!button.GetComponent<ShopButton>().IsLocked)
            {
                button.onClick.AddListener(() =>
                    GameManager.Instance.CurrentKnifeSprite =
                        button.gameObject.transform.GetChild(0).GetComponent<Image>().sprite);
            }

            button.onClick.AddListener(ResetButtonsColor);
            button.onClick.AddListener(() => button.GetComponent<Image>().color = selectedColor);
            button.onClick.AddListener(() => selectedButton = button.GetComponent<ShopButton>());
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
        rectTransform = GetComponent<RectTransform>();
        var selectdId = PlayerPrefs.GetInt("knifeID", 0);

        selectedButton = knifesButtons[selectdId].GetComponent<ShopButton>();
        knifesButtons[selectdId].GetComponent<Image>().color = selectedColor;
    }

    void Update()
    {
        if (swipeCoroutine == null)
        {
            if (Input.touchCount > 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    var prevNumber = currentNumber;
                    if (Mathf.Abs(Input.GetTouch(0).deltaPosition.x) > swipeDistance)
                    {
                        if (Input.GetTouch(0).deltaPosition.x > 0)
                        {
                            if (currentNumber > 0)
                            {
                                swipeCoroutine = StartCoroutine(PageMove(800));

                                currentNumber--;
                            }
                        }
                        else
                        {
                            if (currentNumber < maxNumber)
                            {
                                swipeCoroutine = StartCoroutine(PageMove(-800));
                                currentNumber++;
                            }
                        }
                    }

                    slidersRenderers[prevNumber].color = Color.black;
                    slidersRenderers[currentNumber].color = selectedColor;
                }
            }
        }
    }

    #endregion


    public ShopButton GetSelectedButton()
    {
        return selectedButton;
    }

    public List<int> GetUnlockedButtons()
    {
        List<int> unlockedIds = new List<int>();
        if (knifesButtons != null)
        {
            foreach (var button in knifesButtons)
            {
                if (!button.GetComponent<ShopButton>().IsLocked)
                {
                    unlockedIds.Add(knifesButtons.IndexOf(button));
                }
            }

            return unlockedIds;
        }

        return null;
    }

    private void UnlockSelected()
    {
        if (selectedButton && GameManager.Instance.UnlockKnife())
            selectedButton.UnlockItem();
    }

    private void ResetButtonsColor()
    {
        foreach (var button in knifesButtons)
        {
            button.GetComponent<Image>().color = Color.black;
        }
    }
    
    private IEnumerator PageMove(int offset)
    {
        Vector2 targetpos = new Vector2(rectTransform.anchoredPosition.x + offset,
            rectTransform.anchoredPosition.y);

        while (Vector2.Distance(rectTransform.anchoredPosition, targetpos) > 0.01f)
        {
            rectTransform.anchoredPosition =
                Vector2.MoveTowards(rectTransform.anchoredPosition, targetpos, 50f);
            yield return null;
        }

        rectTransform.anchoredPosition = targetpos;
        swipeCoroutine = null;
    }
}