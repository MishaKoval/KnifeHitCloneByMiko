using UnityEngine;
[CreateAssetMenu(fileName = "Shop Item", menuName = "Gameplay/New Shop Item")]
public class ShopItem:ScriptableObject
{
    [SerializeField] private Sprite unlockedSprite;
    [SerializeField] private Sprite lockedSprite;

    public Sprite GetSprite(bool isLocked)
    {
        if (!isLocked)
        {
            return unlockedSprite;
        }
        return lockedSprite;
    }
}