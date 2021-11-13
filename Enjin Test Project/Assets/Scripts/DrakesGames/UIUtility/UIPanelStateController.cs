using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class UIPanelStateController : MonoBehaviour
{
    [Header("Startup")] 
    [SerializeField] private GameObject[] activeObjectsOnStartup;
    [SerializeField] private GameObject[] inactiveObjectsOnStartup;

    private void Awake()
    {
        foreach (var obj in activeObjectsOnStartup)
        {
            obj.SetActive(true);
        }
        
        foreach (var obj in inactiveObjectsOnStartup)
        {
            obj.SetActive(false);
        }
    }
}
