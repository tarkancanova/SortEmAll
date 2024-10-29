using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    [Header("Slot Settings")]
    public int slotNumberInShelf;
    public GameObject currentItem;
    public bool isCascaded = false;

    [Header("Visual Settings")]
    [Tooltip("Color multiplier at home.")]
    [Range(0f, 1f)]
    public float cascadedColorMultiplier = 0.6f;

    private ItemVisuals itemVisuals;

    private void Awake()
    {
        if (currentItem != null)
        {
            InitializeItem(currentItem);
        }
    }

    public bool IsEmpty()
    {
        return currentItem == null;
    }

    public string GetItemTag()
    {
        return currentItem != null ? currentItem.tag : null;
    }

    public void ClearSlot()
    {
        if (currentItem != null)
        {
            Destroy(currentItem);
            currentItem = null;
        }

    }

    public void PlaceItem(GameObject item)
    {
        if (item == null)
        {
            return;
        }

        if (currentItem != null)
        {
            ClearSlot();
        }

        currentItem = item;
        currentItem.transform.SetParent(transform);
        currentItem.transform.localPosition = Vector2.zero;
        currentItem.transform.localRotation = Quaternion.identity;

        InitializeItem(currentItem);
    }

    private void InitializeItem(GameObject item)
    {
        itemVisuals = item.GetComponent<ItemVisuals>();
        if (itemVisuals == null)
        {
            Debug.LogWarning($"[Slot: {gameObject.name}] ItemVisuals component missing on {item.name}.");
        }
        else
        {
            Debug.Log($"[Slot: {gameObject.name}] Initialized ItemVisuals for {item.name}.");
        }

        if (isCascaded)
        {
            ApplyCascadedVisuals();
        }
        else
        {
            ApplyPrimaryVisuals();
        }
    }

    public void ApplyCascadedVisuals()
    {
        if (itemVisuals != null)
        {
            itemVisuals.ApplyCascadedVisuals(cascadedColorMultiplier);
        }
    }

    public void ApplyPrimaryVisuals()
    {
        if (itemVisuals != null)
        {
            itemVisuals.ApplyPrimaryVisuals();
        }
    }

    public void SetAsCascaded()
    {
        isCascaded = true;
        if (currentItem != null)
        {
            ApplyCascadedVisuals();
        }
    }

    public void SetAsPrimary()
    {
        isCascaded = false;
        if (currentItem != null)
        {
            ApplyPrimaryVisuals();
        }
    }
}