using UnityEngine;

public class ButtonsController : MonoBehaviour
{
    public static ButtonsController Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void OnInstagramButtonClick()
    {
        Application.OpenURL("https://www.instagram.com/miko_nskiy/");
    }
    
    public void OnFacebookButtonClick()
    {
        Application.OpenURL("https://www.facebook.com/profile.php?id=100010718467634");
    }
    
    public void OnDontHaveApples()
    {
        ShowToast("You dont have apples!");
    }
    private void ShowToast(string toastMessage)
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity, toastMessage, 0);
                toastObject.Call("show");
            }));
        }
    }
    public void OnDontUseButtonClick()
    {
        ShowToast("IN DEVELOP!:)");
    }
    
}
