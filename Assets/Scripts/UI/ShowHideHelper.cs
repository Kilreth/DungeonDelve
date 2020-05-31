using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Assign this to every UI object which shows and hides in various menus.
/// </summary>
public class ShowHideHelper : MonoBehaviour
{
    // If controlling a UI object that should be disabled at times
    // (eg. the Continue button when no save exists), assign this in
    // the inspector and implement the logic in DetermineInteractable().
    public Selectable Selectable;

    [SerializeField]
    private string[] showOnStates = null;

    private void DetermineInteractable()
    {
        if (Selectable == null)
            return;

        if (gameObject.name == "Continue")
            Selectable.interactable = SaveGameSystem.DoesSaveGameFileExist();
    }

    public void ShowOrHide(string newState)
    {
        foreach (string state in showOnStates)
        {
            if (state == newState)
            {
                gameObject.SetActive(true);
                DetermineInteractable();
                return;
            }
        }
        gameObject.SetActive(false);
    }
}
