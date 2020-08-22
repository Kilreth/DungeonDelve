using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Adapted from domussolisortum's script at
// http://answers.unity.com/answers/1294940/view.html
public class ImageHighlight : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private Image image;

    [SerializeField]
    private Color normalColor;
    [SerializeField]
    private Color highlightedColor;
    [SerializeField]
    private Color pressedColor;

    void OnEnable()
    {
        image.color = normalColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.color = highlightedColor;
        Jukebox.Instance.PlaySFX("UI highlight");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        image.color = pressedColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        image.color = highlightedColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.color = normalColor;
    }
}
