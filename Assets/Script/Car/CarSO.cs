using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
[CreateAssetMenu(fileName ="New CarSO", menuName = "ScriptableObjects/CarSO")]
public class CarSO : ScriptableObject
{
    public float speed;
    public float acceleration;
}
