using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Video;

public class HttpList : MonoBehaviour
{
    public GameObject buttonPrefab;
    public Transform buttonContainer;
    public VideoPlayer videoPlayer;
    public GameObject canvasMenu, CanvasVideo;

    private List<GameObject> buttons = new List<GameObject>();
    private string previousUrl;

    private int retryCount = 0; // Agrega esta variable para llevar la cuenta de cuántas veces se ha intentado reproducir el video

    void Start()
    {
        StartCoroutine(GetServerData("http://gluvu.atspace.cc/Passion%20Wii%20Streaming"));
        ButtonZonaKids();
        //StartCoroutine(GetServerData("http://gluvu.atspace.cc/Zona%20Kids"));
        videoPlayer.prepareCompleted += OnVideoPrepared; // Agrega este evento para saber cuándo el video está listo para ser reproducido
    }

    IEnumerator GetServerData(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log("Error: " + webRequest.error);
            }
            else
            {
                string[] lines = webRequest.downloadHandler.text.Split('\n');
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

    public void ButtonZonaKids()
    {
        CreateButton("Zona Kids", "http://gluvu.atspace.cc/Zona%20Kids");
    }

    public void OnButtonClick(string url)
    {
        if (url.Contains(".mp4") || url.Contains(".mkv"))
        {
            videoPlayer.url = url;
            videoPlayer.Prepare(); // Carga el video para obtener su información
            videoPlayer.Play();
            videoPlayer.errorReceived += OnVideoError;
            
            canvasMenu.SetActive(false);
            CanvasVideo.SetActive(true);
            //Application.OpenURL(url);
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

    void OnVideoError(VideoPlayer source, string message)
    {
        if (retryCount < 3) // Si se ha intentado reproducir el video menos de 3 veces
        {
            Debug.LogError("Error al reproducir el video: Reintentando...");
            retryCount++; // Incrementa la cuenta de intentos
            videoPlayer.Prepare(); // Carga el video en el buffer para obtener su información
        }
        else // Si se ha intentado reproducir el video 3 veces o más
        {
            CanvasVideo.SetActive(false);
            canvasMenu.SetActive(true);
            videoPlayer.Stop();
            Debug.LogError("Error al reproducir el video: " + message);
            retryCount = 0; // Reinicia la cuenta de intentos
        }
    }
    void OnVideoPrepared(VideoPlayer source)
    {
        videoPlayer.Play(); // Reproduce el video cuando está listo
    }
}