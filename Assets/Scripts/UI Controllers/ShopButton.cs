using System;
using UnityEngine;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour
{
    [SerializeField] private ShopItem shopItem;
    private bool isLocked = true;

    public bool IsLocked => isLocked;
    private void Awake()
    {
        ChangeSprite();
    }

    private void ChangeSprite()
    {
        if (shopItem)
        {
            gameObject.transform.GetChild(0).GetComponent<Image>().sprite = shopItem.GetSprite(isLocked);
            gameObject.transform.GetChild(0).GetComponent<Image>().SetNativeSize();
        }
    }

    public void UnlockItem()
    {
        isLocked = false;
        ChangeSprite();
    }
}