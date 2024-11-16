using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private TMP_Text time;
    [SerializeField] private TMP_Text StartPoint;
    [SerializeField] private TMP_Text EndPoint;
    void Start()
    {
        time.text = "ETM : ";
        StartPoint.text = "Start: ";
        EndPoint.text = "End: ";
    }
}
