using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Manages the drag-and-drop functionality for items within the game,
/// allowing players to move items between slots seamlessly.
/// </summary>
public class DragAndDropUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private Vector2 originalPosition;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("[DragAndDrop] Canvas not found in parent hierarchy.");
        }

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        originalPosition = rectTransform.anchoredPosition;

        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canvas == null) return;

        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        transform.SetParent(canvas.transform, true);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        GameObject dropTarget = eventData.pointerCurrentRaycast.gameObject;

        if (dropTarget != null && dropTarget.CompareTag("DropTarget"))
        {
            Slot targetSlot = dropTarget.GetComponent<Slot>();
            Slot originalSlot = originalParent.GetComponent<Slot>();

            if (targetSlot != null && targetSlot.IsEmpty())
            {
                targetSlot.PlaceItem(gameObject);
                targetSlot.ApplyPrimaryVisuals();
                originalSlot.currentItem = null;

                EventManager.Instance.ItemPlaced(targetSlot);
            }
            else
            {
                ResetPosition();
            }
        }
        else
        {
            ResetPosition();
        }
    }

    private void ResetPosition()
    {
        transform.SetParent(originalParent);
        rectTransform.anchoredPosition = originalPosition;
        Debug.Log($"[DragAndDrop] Reset position of {gameObject.name} to {originalParent.name}.");
    }
}
