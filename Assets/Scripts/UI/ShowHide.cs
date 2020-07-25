using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Assign this to the Canvas, and call SetState() for every menu transition.
/// </summary>
public class ShowHide : MonoBehaviour
{
    [SerializeField]
    private string defaultState;
    [SerializeField]
    private string[] matchWidthStates;
    [SerializeField]
    private string[] matchHeightStates;
    private CanvasScaler canvasScaler;

    void Awake()
    {
        canvasScaler = transform.GetComponent<CanvasScaler>();
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

        if (matchHeightStates.Contains(state))
            canvasScaler.matchWidthOrHeight = 1;
        else if (matchWidthStates.Contains(state))
            canvasScaler.matchWidthOrHeight = 0;
    }
}
