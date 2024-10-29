using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Shelf : MonoBehaviour
{
    [Header("Shelf Settings")]
    public List<Slot> slots = new List<Slot>();

    private void Awake()
    {
        InitializeSlots();
    }

    private void InitializeSlots()
    {
        foreach (Transform child in transform)
        {
            Slot slot = child.GetComponent<Slot>();
            if (slot != null)
            {
                slots.Add(slot);
            }
        }

        if (slots.Count != 6)
        {
            Debug.LogError($"Shelf {gameObject.name} does not have exactly 6 slots. Current count: {slots.Count}");
        }
        else
        {
            CategorizeSlots();
        }
    }

    private void CategorizeSlots()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (i < 3)
            {
                slots[i].SetAsPrimary();
            }
            else
            {
                slots[i].SetAsCascaded();
            }
        }
    }


    public List<Slot> GetNonCascadedSlots()
    {
        return slots.Take(3).ToList();
    }


    public List<Slot> GetCascadedSlots()
    {
        return slots.Skip(3).Take(3).ToList();
    }


    public bool CheckForMatches()
    {
        int matchLength = 3;

        List<Slot> nonCascadeSlots = GetNonCascadedSlots();

        if (nonCascadeSlots.Count < matchLength)
        {
            return false;
        }

        bool matchFound = false;

        for (int i = 0; i <= nonCascadeSlots.Count - matchLength; i++)
        {
            bool isMatch = true;
            string firstTag = nonCascadeSlots[i].GetItemTag();

            if (string.IsNullOrEmpty(firstTag))
                continue;

            for (int j = 1; j < matchLength; j++)
            {
                if (nonCascadeSlots[i + j].IsEmpty() || nonCascadeSlots[i + j].GetItemTag() != firstTag)
                {
                    isMatch = false;
                    break;
                }
            }

            if (isMatch)
            {
                Debug.Log($"Match found on {gameObject.name} at slots {i} to {i + matchLength - 1}.");

                for (int j = 0; j < matchLength; j++)
                {
                    nonCascadeSlots[i + j].ClearSlot();
                }

                matchFound = true;

                i = -1;
            }
        }

        return matchFound;
    }
}
