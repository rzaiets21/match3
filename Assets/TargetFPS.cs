using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFPS : MonoBehaviour
{
    [SerializeField] private int targetFrameRate = 60;
    
    private void Start()
    {
        Application.targetFrameRate = targetFrameRate;
    }
}
