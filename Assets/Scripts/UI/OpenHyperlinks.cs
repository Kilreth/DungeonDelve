using UnityEngine;

public class OpenHyperlinks : MonoBehaviour
{
    public void OpenURL(string url)
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            Application.ExternalEval("window.open('" + url + "', '_blank')");
        }
        else
        {
            Application.OpenURL(url);
        }
    }
}
