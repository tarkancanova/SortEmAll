using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public event Action<Shelf> OnMatchCleared;

    public void MatchCleared(Shelf shelf)
    {
        OnMatchCleared?.Invoke(shelf);
    }


    public event Action<Slot> OnItemPlaced;

    public void ItemPlaced(Slot slot)
    {
        OnItemPlaced?.Invoke(slot);
    }
}
