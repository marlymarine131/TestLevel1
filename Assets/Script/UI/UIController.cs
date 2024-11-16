using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private TMP_Text time;
    [SerializeField] private TMP_Text StartAPoint;
    [SerializeField] private TMP_Text EndPoint;
    [SerializeField] private TMP_Text StartBPoint;

    [SerializeField] private GameManager gameManager;
    void Start()
    {
        time.text = "ETM : ";
        StartAPoint.text = "Start A: ";
        StartBPoint.text = "Start B: ";
        EndPoint.text = "End: ";

        gameManager.onStartAChange += OnStartAChangeEventHandler;
        gameManager.onStartBChange += OnStartBChangeEventHandler;
        gameManager.onEndChange += OnEndPointChangeEventHandler;

    }

    private void OnEndPointChangeEventHandler(object sender, GameManager.OnEndChange_Args e)
    {
        UpdateEnd(e.endName);
    }

    private void OnStartBChangeEventHandler(object sender, GameManager.OnStartBChange_Args e)
    {
        UpdateStartB(e.startBName);
    }

    private void OnStartAChangeEventHandler(object sender, GameManager.OnStartAChange_Args e)
    {
        UpdateStartA(e.startAName);
    }

    private void UpdateStartA(string name)
    {
        StartAPoint.text = "start A: " + name;
    }
    private void UpdateStartB(string name)
    {
        StartBPoint.text = "start B:: " + name;
    }
    private void UpdateEnd(string name)
    {
        EndPoint.text = "End Point: " + name;
    }
}
