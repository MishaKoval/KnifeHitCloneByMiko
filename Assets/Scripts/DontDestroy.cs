using System.Security.Cryptography;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{

    private GameObject instance = null;
    
    
    private void Awake()
    {
        
        if(instance == null)
        {
            instance = this.gameObject;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        
    }
}
