using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Satellite : MonoBehaviour
{
    public GameObject satelliteCamera;
    public GameObject satelliteImage;
    public GameObject activator;
    public GameObject countdown;

    public int satelliteDelay;

    int currentTurn;

    bool isScanning = false;
    bool stop = false;
    
    void Start()
    {
        currentTurn = -5;
        countdown.SetActive(false);
        satelliteCamera.SetActive(true);
        satelliteImage.SetActive(false);
        activator.SetActive(true);
    }

    
    void Update()
    {
        if(TurnManager.Instance.turn == currentTurn + satelliteDelay && TurnManager.Instance.currentTeam == 1)
        {
            satelliteImage.SetActive(true);
            isScanning = false;
        }
        else
        {
           if(!isScanning)
           {
                countdown.SetActive(false);
                activator.SetActive(true);
                satelliteCamera.SetActive(true);
                satelliteImage.SetActive(false);
           }
        }

        if(isScanning)
        {
            countdown.GetComponent<TextMeshProUGUI>().text = ((TurnManager.Instance.turn - currentTurn - satelliteDelay) * -1).ToString();
        }
        else
        {
            countdown.GetComponent<TextMeshProUGUI>().text = 0.ToString();
        }
    }

    public void Scan()
    {
        currentTurn = TurnManager.Instance.turn;
        activator.SetActive(false);
        satelliteCamera.SetActive(false);
        isScanning = true;
        countdown.SetActive(true);
    }

    public void Close()
    {
        currentTurn = -5;
        stop = true;
        isScanning = false;
        satelliteImage.SetActive(false);
    }
}
