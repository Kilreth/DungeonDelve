using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowHideHelper : MonoBehaviour
{
    [System.Serializable]
    public class ShowOnState
    {
        public string state;
    }

    // If controlling a UI object that should be disabled at times
    // (eg. the Continue button when no save exists), assign this in
    // the inspector and implement the logic in DetermineInteractable().
    public Selectable Selectable;

    [SerializeField]
    private ShowOnState[] showOnStates = null;

    private void DetermineInteractable()
    {
        if (Selectable == null)
            return;

        if (gameObject.name == "Continue")
        {
            if (SaveGameSystem.DoesSaveGameFileExist())
                Selectable.interactable = true;
            else
                Selectable.interactable = false;
        }
    }

    public void ShowOrHide(string state)
    {
        foreach (ShowOnState showOnState in showOnStates)
        {
            if (state == showOnState.state)
            {
                gameObject.SetActive(true);
                DetermineInteractable();
                return;
            }
        }
        gameObject.SetActive(false);
    }
}
