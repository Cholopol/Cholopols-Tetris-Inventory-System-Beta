using UnityEngine;
using UnityEngine.UI;

public class MultiURLController : MonoBehaviour
{
    [System.Serializable]
    public class ButtonURLPair
    {
        public Button button;
        public string targetURL;
    }

    public ButtonURLPair[] buttonLinks = new ButtonURLPair[4];

    void Start()
    {
        if (buttonLinks.Length != 4)
        {
            Debug.LogError("Need to configure 4 Button-URL correspondences£¡");
            return;
        }

        foreach (var pair in buttonLinks)
        {
            if (pair.button != null)
            {
                pair.button.onClick.AddListener(() => OpenURL(pair.targetURL));
            }
            else
            {
                Debug.LogWarning("Exist unconfigured button references£¡");
            }
        }
    }

    void OpenURL(string url)
    {
        if (!string.IsNullOrEmpty(url))
        {
            Application.OpenURL(url);
            Debug.Log($"Openning£º{url}");
        }
        else
        {
            Debug.LogWarning("URL is null, please check the configuration£¡");
        }
    }
}