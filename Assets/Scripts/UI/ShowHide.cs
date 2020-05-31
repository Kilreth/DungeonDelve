using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Assign this to the Canvas, and call SetState() for every menu transition.
/// </summary>
public class ShowHide : MonoBehaviour
{
    [SerializeField]
    private string defaultState;

    void Awake()
    {
        SetState(defaultState);
    }

    /// <summary>
    /// Ask each UI object to show or hide according to the new state.
    /// </summary>
    /// <param name="state"></param>
    public void SetState(string state)
    {
        foreach (Transform child in gameObject.transform)
        {
            ShowHideHelper showHideHelper = child.gameObject.GetComponent<ShowHideHelper>();
            if (showHideHelper != null)
            {
                showHideHelper.ShowOrHide(state);
            }
        }
    }
}
