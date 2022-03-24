using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonsController : MonoBehaviour
{
    // Start is called before the first frame update

    //[SerializeField] private GameObject currency;
    
    /*private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }*/

    public void OnPlayButtonClick()
    {
        SceneManager.LoadScene(1);  
    }
    public void OnInstagramButtonClick()
    {
        Application.OpenURL("https://www.instagram.com/miko_nskiy/");
    }

    public void OnHomeButtonClick()
    {
        SceneManager.LoadScene(0);
    }

    public void OnDontUseButtonClick()
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity, "IN DEVELOP!:)", 0);
                toastObject.Call("show");
            }));
        }
    }

    /*private void OnDestroy()
    {
        
    }*/
}
