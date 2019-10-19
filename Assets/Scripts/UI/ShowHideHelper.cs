﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowHideHelper : MonoBehaviour
{
    [System.Serializable]
    public class ShowOnState
    {
        public string state;
    }

    [SerializeField]
    private ShowOnState[] showOnStates;

    public void ShowOrHide(string state)
    {
        foreach (ShowOnState showOnState in showOnStates)
        {
            if (state == showOnState.state)
            {
                gameObject.SetActive(true);
                return;
            }
        }
        gameObject.SetActive(false);
    }
}
