using UnityEngine;

public class ToggleController : MonoBehaviour
{
    [SerializeField] private GameObject[] _objectsToSwitch;
    private void Awake()
    {
        _objectsToSwitch = new GameObject[transform.childCount];
        for (int i = 0; i < _objectsToSwitch.Length; i++)
        {
            _objectsToSwitch[i] = transform.GetChild(i).gameObject;
        }
    }

    public void OnToogleChanged()
    { 
        foreach (var obj in _objectsToSwitch)
        {
            obj.SetActive(!obj.activeSelf);
        }
    }
}
