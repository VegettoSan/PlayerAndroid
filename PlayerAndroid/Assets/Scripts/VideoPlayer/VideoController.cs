using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.EventSystems;

public class VideoController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public GameObject botonPlay, botonPausa, botonReiniciar, botonVolumen, botonMute;
    public bool play, termino;
    public VideoPlayer videoPlayer;
    public Slider barra;
    bool slide = false;
    public AudioSource audioSource;
    public Slider barraVolumen;
    public Text totalDurationText;
    public Text currentDurationText;

    private bool isVideoPlaying = false;

    void Start()
    {
        // Registra un evento que se activará cuando el video haya sido preparado para su reproducción
        videoPlayer.prepareCompleted += OnVideoPrepareCompleted;

        play = true;
        botonReiniciar.SetActive(false);
        videoPlayer.Play();
        videoPlayer.Pause();
    }

    public void OnPointerDown(PointerEventData a)
    {
        videoPlayer.Pause();
        slide = true;
    }
    public void OnPointerUp(PointerEventData a)
    {
        if (play)
        {
            videoPlayer.Play();
        }
        else
        {
            videoPlayer.Pause();
        }
        float frame = (float)barra.value * (float)videoPlayer.frameCount;
        videoPlayer.frame = (long)frame;
        slide = false;
    }

    void Update()
    {
        if(slide == false)
        {
            barra.value = (float)videoPlayer.frame / (float)videoPlayer.frameCount;
        }
        if(barra.value >= 0.99 && termino == false)
        {
            botonReiniciar.SetActive(true);
            botonPlay.SetActive(false);
            botonReiniciar.SetActive(false);
            termino = true;
        }
        if(barra.value == 0)
        {
            termino = false;
        }

        if (isVideoPlaying)
        {
            ShowCurrentVideoDuration();
        }
    }

    //

    public void Volumen()
    {
        audioSource.volume = barraVolumen.value;
    }
    public void Play()
    {
        videoPlayer.Play();
        botonPlay.SetActive(false);
        botonPausa.SetActive(true);
        play = true;
    }
    public void Pausa()
    {
        videoPlayer.Pause();
        botonPlay.SetActive(true);
        botonPausa.SetActive(false);
        play = false;
    }
    public void Reiniciar()
    {
        videoPlayer.frame = 0;
        Play();
        botonReiniciar.SetActive(false);
    }

    //

    public void Silenciar()
    {
        if (audioSource.mute)
        {
            botonMute.SetActive(false);
            botonVolumen.SetActive(true);
            audioSource.mute = false;
        }
        else
        {
            botonMute.SetActive(true);
            botonVolumen.SetActive(false);
            audioSource.mute = true;
        }
    }

    //

    // Este evento se activa cuando el video ha sido preparado para su reproducción
    void OnVideoPrepareCompleted(VideoPlayer source)
    {
        ShowTotalVideoDuration();
        isVideoPlaying = true;
    }

    void ShowTotalVideoDuration()
    {
        ulong totalFrames = videoPlayer.frameCount;
        double totalDurationSeconds = totalFrames / videoPlayer.frameRate;
        int totalDurationMinutes = Mathf.FloorToInt((float)totalDurationSeconds / 60);
        int totalDurationHours = totalDurationMinutes / 60;
        totalDurationMinutes %= 60;
        int totalDurationSecondsRemaining = Mathf.FloorToInt((float)totalDurationSeconds % 60);
        string totalDurationString = string.Format("{0:D2}:{1:D2}:{2:D2}", totalDurationHours, totalDurationMinutes, totalDurationSecondsRemaining);
        totalDurationText.text = totalDurationString;
    }

    void ShowCurrentVideoDuration()
    {
        double currentDurationSeconds = videoPlayer.time;
        int currentDurationMinutes = Mathf.FloorToInt((float)currentDurationSeconds / 60);
        int currentDurationHours = currentDurationMinutes / 60;
        currentDurationMinutes %= 60;
        int currentDurationSecondsRemaining = Mathf.FloorToInt((float)currentDurationSeconds % 60);
        string currentDurationString = string.Format("{0:D2}:{1:D2}:{2:D2}", currentDurationHours, currentDurationMinutes, currentDurationSecondsRemaining);
        currentDurationText.text = currentDurationString;
    }
}

