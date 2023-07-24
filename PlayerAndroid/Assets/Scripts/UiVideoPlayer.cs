using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UiVideoPlayer : MonoBehaviour {

    public GameObject UI;
    public float timeToDeactivate = 3f;
    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= timeToDeactivate)
        {
            UI.SetActive(false);
        }
    }

    public void ResetTime()
    {
        UI.SetActive(true);
        timer = 0;
    }
}
