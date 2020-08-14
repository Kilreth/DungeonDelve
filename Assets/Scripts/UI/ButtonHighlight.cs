using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

// Adapted from domussolisortum's script at
// http://answers.unity.com/answers/1294940/view.html
[RequireComponent(typeof(Button))]
public class ButtonHighlight : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler
{
    private Button button;
    private TextMeshProUGUI text;

    [SerializeField]
    private Color normalColor;
    [SerializeField]
    private Color highlightedColor;
    [SerializeField]
    private Color pressedColor;

    void Awake()
    {
        button = GetComponent<Button>();
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    void OnEnable()
    {
        if (button.interactable)
        {
            text.color = normalColor;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button.interactable)
        {
            text.color = highlightedColor;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (button.interactable)
        {
            text.color = pressedColor;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (button.interactable)
        {
            text.color = highlightedColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (button.interactable)
        {
            text.color = normalColor;
        }
    }
}
