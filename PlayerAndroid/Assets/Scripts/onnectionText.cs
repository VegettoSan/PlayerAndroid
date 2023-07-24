using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class onnectionText : MonoBehaviour {

    public Text connectionText;

    IEnumerator Start()
    {
        while (true)
        {
            switch (Application.internetReachability)
            {
                case NetworkReachability.NotReachable:
                    connectionText.text = "Error: Sin conexión a Internet";
                    break;
                case NetworkReachability.ReachableViaCarrierDataNetwork:
                case NetworkReachability.ReachableViaLocalAreaNetwork:
                    using (UnityWebRequest www = UnityWebRequest.Get("http://www.google.com"))
                    {
                        yield return www.SendWebRequest();

                        if (www.isNetworkError || www.isHttpError)
                        {
                            connectionText.text = "Error: No se puede conectar a Internet" + www.error;
                            Debug.Log("Error: " + www.error);
                        }
                        else
                        {
                            connectionText.text = "Conectado a Internet";
                        }
                    }
                    break;
            }

            yield return new WaitForSeconds(5);
        }
    }
}