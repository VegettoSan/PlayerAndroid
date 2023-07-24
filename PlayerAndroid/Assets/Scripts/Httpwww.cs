using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.Video;

public class Httpwww : MonoBehaviour
{
    public GameObject buttonPrefab;
    public Transform buttonContainer;
    //public VideoPlayer videoPlayer;
    //public GameObject canvasMenu, CanvasVideo;

    private List<GameObject> buttons = new List<GameObject>();
    private string previousUrl;

    void Start()
    {
        StartCoroutine(GetServerData("http://gluvu.atspace.cc/Passion%20Wii%20Streaming"));
    }

    IEnumerator GetServerData(string url)
    {
        using (WWW webRequest = new WWW(url))
        {
            yield return webRequest;

            if (!string.IsNullOrEmpty(webRequest.error))
            {
                Debug.Log("Error: " + webRequest.error);
            }
            else
            {
                string[] lines = webRequest.text.Split('\n');
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].StartsWith("#EXTM3U"))
                    {
                        CreateButton("...", previousUrl);
                    }
                    else if (lines[i].StartsWith("#EXTINF:-1,") || lines[i].StartsWith("#EXTINF:-1 ,")
                        || lines[i].StartsWith("#EXTINF-1,") || lines[i].StartsWith("#EXTINF-1 ,"))
                    {
                        string name = lines[i].Split(',')[1];
                        string buttonUrl = lines[i + 1];

                        CreateButton(name, buttonUrl);
                    }
                }
            }
        }

        previousUrl = url;
    }

    void CreateButton(string name, string url)
    {
        if (!name.Contains("Wii") && !name.Contains("wii"))
        {
            GameObject buttonObj = Instantiate(buttonPrefab, buttonContainer);
            buttonObj.GetComponentInChildren<Text>().text = name;
            buttonObj.GetComponent<Button>().onClick.AddListener(() => OnButtonClick(url));
            buttons.Add(buttonObj);
        }
    }

    public void OnButtonClick(string url)
    {
        if (url.Contains(".mp4") || url.Contains(".mkv"))
        {
            /*videoPlayer.url = url;
            videoPlayer.Play();
            videoPlayer.errorReceived += OnVideoError;

            canvasMenu.SetActive(false);
            CanvasVideo.SetActive(true);*/
            Application.OpenURL(url);
        }
        else
        {
            foreach (GameObject button in buttons)
            {
                Destroy(button);
            }
            buttons.Clear();

            StartCoroutine(GetServerData(url));
        }
    }

    /*void OnVideoError(VideoPlayer source, string message)
    {
        CanvasVideo.SetActive(false);
        canvasMenu.SetActive(true);
        videoPlayer.Stop();
        Debug.LogError("Error al reproducir el video: " + message);
    }*/
}
