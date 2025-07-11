﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class stores relevant information about a page of UI
/// </summary>
public class UIPage : MonoBehaviour
{
    [Tooltip("The default UI to have selected when opening this page")]
    public GameObject defaultSelected;

    /// <summary>
    /// Description:
    /// Sets the currently selected UI to the one defaulted by this UIPage
    /// Input:
    /// none
    /// Return:
    /// void (no return)
    /// </summary>
    public void SetSelectedUIToDefault()
    {
        if (UIManager.instance != null && defaultSelected != null)
        {
            UIManager.instance.eventSystem.SetSelectedGameObject(null);
            UIManager.instance.eventSystem.SetSelectedGameObject(defaultSelected);
        }

    }
}