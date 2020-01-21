using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowHide : MonoBehaviour
{
    private static string state;

    void Awake()
    {
        SetState("Default");
    }

    public void SetState(string newState)
    {
        state = newState;
        ShowAndHideItems(gameObject.transform);
    }

    public void ShowAndHideItems(Transform parent)
    {
        foreach (Transform child in parent)
        {
            ShowHideHelper showHideData = child.gameObject.GetComponent<ShowHideHelper>();
            if (showHideData != null)
            {
                showHideData.ShowOrHide(state);
            }
            ShowAndHideItems(child);
        }
    }
}