using UnityEngine;

public class OpenHyperlinks : MonoBehaviour
{
    public void OpenURL(string url)
    {
        Application.OpenURL(url);
    }
}
