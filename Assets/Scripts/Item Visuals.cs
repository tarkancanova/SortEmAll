using UnityEngine;
using UnityEngine.UI;

public class ItemVisuals : MonoBehaviour
{
    private Image itemImage;
    private Shadow itemShadow;
    private Color originalColor;

    private void Awake()
    {
        itemImage = GetComponent<Image>();
        if (itemImage == null)
        {
            Debug.LogWarning($"[ItemVisuals] No Image component found on {gameObject.name}.");
        }
        else
        {
            originalColor = itemImage.color;
        }

        itemShadow = GetComponent<Shadow>();
        if (itemShadow == null)
        {
            itemShadow = gameObject.AddComponent<Shadow>();
            itemShadow.effectColor = Color.black;
            itemShadow.effectDistance = new Vector2(2, -2);
        }
    }


    public void ApplyPrimaryVisuals()
    {
        if (itemImage != null)
        {
            itemImage.color = originalColor;

        }

        transform.localScale = Vector3.one;


        if (itemShadow != null)
        {
            itemShadow.effectColor = Color.clear;

        }
    }

    /// <param name="colorMultiplier">Multiplier to darken the item's color.</param>
    public void ApplyCascadedVisuals(float colorMultiplier)
    {
        if (itemImage != null)
        {
            itemImage.color = new Color(
                originalColor.r * colorMultiplier,
                originalColor.g * colorMultiplier,
                originalColor.b * colorMultiplier,
                originalColor.a
            );
        }

        transform.localScale = new Vector3(0.8f, 0.8f, 1f);


        if (itemShadow != null)
        {
            itemShadow.effectColor = Color.black;
            itemShadow.effectDistance = new Vector2(2, -2);
        }
    }
}
