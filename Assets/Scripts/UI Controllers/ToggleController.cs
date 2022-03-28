using UnityEngine;

public class ToggleController : MonoBehaviour
{
    [SerializeField] private GameObject[] objectsToSwitch;
    private void Awake()
    {
        objectsToSwitch = new GameObject[transform.childCount];
        for (int i = 0; i < objectsToSwitch.Length; i++)
        {
            objectsToSwitch[i] = transform.GetChild(i).gameObject;
        }
    }

    public void OnToogleChanged()
    { 
        foreach (var obj in objectsToSwitch)
        {
            obj.SetActive(!obj.activeSelf);
        }
    }
}
