using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    public int level;

    [Header("Scriptable Object")]
    public PlayerData playerData;


    [Header("Prefabs")]
    public GameObject shelfPrefab;

    [Header("Item Prefabs")]
    public List<GameObject> itemPrefabs;

    [Header("Slot Management")]
    public List<Shelf> shelves = new List<Shelf>();
    public List<GameObject> addedItems = new List<GameObject>();

    [Header("Score Management")]
    public int score = 0;
    public int scorePerMatch = 10;
    public int scorePerCascade = 5;
    public int matchCounter;
    public float comboMultiplier = 1f;
    public float comboDuration = 5f;
    private float lastMatchTime;

    [Header("UI Elements")]
    public TMP_Text scoreText;

    private void Awake()
    {
        level = playerData.playerLevel;
    }

    private void Start()
    {
        InitializeGame();
        UpdateScoreUI();
        matchCounter = 0;
    }

    private void InitializeGame()
    {
        GenerateShelves(level);
        AssignItemsInSetsOfTwelve();
    }

    private void GenerateShelves(int level)
    {
        int shelvesToGenerate = level * 3;

        for (int i = 0; i < shelvesToGenerate; i++)
        {
            GameObject instantiatedShelf = Instantiate(shelfPrefab, this.transform);
            Shelf shelfComponent = instantiatedShelf.GetComponent<Shelf>();

            if (shelfComponent != null)
            {
                shelves.Add(shelfComponent);
            }
        }
    }

    private void AssignItemsInSetsOfTwelve()
    {
        if (shelves == null || shelves.Count == 0)
        {
            return;
        }

        int itemCount = 12 * level;

        if (itemPrefabs.Count < itemCount)
        {
            return;
        }

        List<GameObject> selectedItems = itemPrefabs.Take(itemCount).ToList();

        for (int i = 0; i < 3; i++)
        {
            if (i >= shelves.Count)
            {
                return;
            }

            List<Slot> nonCascadedSlots = shelves[i].GetNonCascadedSlots();
            Slot targetSlot = nonCascadedSlots.Find(slot => slot.IsEmpty());

            if (targetSlot != null)
            {
                GameObject newItem = Instantiate(selectedItems[i]);
                newItem.GetComponent<RectTransform>().localScale = Vector3.one;
                targetSlot.PlaceItem(newItem);
                addedItems.Add(newItem);

                DragAndDropUI dragAndDropUI = newItem.GetComponent<DragAndDropUI>();
                if (dragAndDropUI != null)
                {
                    dragAndDropUI.enabled = true; 
                }
            }
            else
            {
                return;
            }
        }

        if (shelves.Count > 0)
        {
            Shelf targetShelf = shelves[0];

            List<Slot> nonCascadedSlots = targetShelf.GetNonCascadedSlots().Where(slot => slot.IsEmpty()).ToList();
            if (nonCascadedSlots.Count >= 2)
            {
                for (int i = 3; i < 5; i++)
                {
                    Slot targetSlot = nonCascadedSlots[0];
                    nonCascadedSlots.RemoveAt(0);

                    GameObject newItem = Instantiate(selectedItems[i]);
                    newItem.GetComponent<RectTransform>().localScale = Vector3.one;
                    targetSlot.PlaceItem(newItem);
                    addedItems.Add(newItem);

                    DragAndDropUI dragAndDropUI = newItem.GetComponent<DragAndDropUI>();
                    if (dragAndDropUI != null)
                    {
                        dragAndDropUI.enabled = true; 
                    }
                }
            }
            else
            {
                List<Slot> cascadedSlots = targetShelf.GetCascadedSlots().Where(slot => slot.IsEmpty()).ToList();
                if (cascadedSlots.Count >= 2)
                {
                    for (int i = 3; i < 5; i++)
                    {
                        Slot targetSlot = cascadedSlots[0];
                        cascadedSlots.RemoveAt(0);

                        GameObject newItem = Instantiate(selectedItems[i]);
                        newItem.GetComponent<RectTransform>().localScale = Vector3.one;
                        targetSlot.PlaceItem(newItem);
                        addedItems.Add(newItem);

                        DragAndDropUI dragAndDropUI = newItem.GetComponent<DragAndDropUI>();
                        if (dragAndDropUI != null)
                        {
                            dragAndDropUI.enabled = false;
                        }
                    }
                }
                else
                {
                    return;
                }
            }
        }

        List<GameObject> remainingItems = selectedItems.Skip(5).ToList();

        List<Slot> allAvailableSlots = new List<Slot>();
        foreach (var shelf in shelves)
        {
            allAvailableSlots.AddRange(shelf.slots.Where(slot => slot.IsEmpty()));
        }

        List<Slot> shuffledSlots = allAvailableSlots.OrderBy(x => Random.value).ToList();

        foreach (var item in remainingItems)
        {
            if (shuffledSlots.Count == 0)
            {
                Debug.LogError("Not enough slots available to place all remaining items.");
                return;
            }

            Slot targetSlot = shuffledSlots[0];
            shuffledSlots.RemoveAt(0);

            GameObject newItem = Instantiate(item);
            newItem.GetComponent<RectTransform>().localScale = Vector3.one;
            targetSlot.PlaceItem(newItem);
            addedItems.Add(newItem);

            if (targetSlot.isCascaded)
            {
                DragAndDropUI dragAndDropUI = newItem.GetComponent<DragAndDropUI>();
                if (dragAndDropUI != null)
                {
                    dragAndDropUI.enabled = false;
                }
            }
            else
            {
                DragAndDropUI dragAndDropUI = newItem.GetComponent<DragAndDropUI>();
                if (dragAndDropUI != null)
                {
                    dragAndDropUI.enabled = true;
                }
            }
        }
    }



    private void OnEnable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnMatchCleared += HandleMatchCleared;
            EventManager.Instance.OnItemPlaced += HandleItemPlaced;
        }
    }

    private void OnDisable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnMatchCleared -= HandleMatchCleared;
            EventManager.Instance.OnItemPlaced -= HandleItemPlaced;
        }
    }

    private void HandleMatchCleared(Shelf shelf)
    {
        CascadeItems(shelf);
        HandleScore();
        UpdateScoreUI();
        matchCounter++;
        CheckWinCondition();
    }

    private void CheckWinCondition()
    {
        if (matchCounter == 4 * level)
        {
            if (playerData.playerLevel < 5) playerData.playerLevel++;
            else playerData.playerLevel = 1;

            SceneManager.LoadScene(0);
        }
    }

    private void HandleItemPlaced(Slot slot)
    {
        Shelf shelf = slot.GetComponentInParent<Shelf>();
        if (shelf != null)
        {
            bool matchDetected = shelf.CheckForMatches();

            if (matchDetected)
            {
                EventManager.Instance.MatchCleared(shelf);
            }
        }
    }

    private void CascadeItems(Shelf shelf)
    {
        if (shelf == null)
        {
            return;
        }

        // Check if all primary (non-cascaded) slots are empty
        bool allPrimaryEmpty = shelf.GetNonCascadedSlots().All(slot => slot.IsEmpty());

        // Check if there are items in cascaded slots
        bool hasCascadedItems = shelf.GetCascadedSlots().Any(slot => !slot.IsEmpty());

        if (allPrimaryEmpty && hasCascadedItems)
        {
            // Get all cascaded slots with items and shuffle them
            List<Slot> cascadedSlots = shelf.GetCascadedSlots().Where(slot => !slot.IsEmpty()).OrderBy(x => Random.value).ToList();
            // Get all empty primary slots and shuffle them
            List<Slot> primarySlots = shelf.GetNonCascadedSlots().Where(slot => slot.IsEmpty()).OrderBy(x => Random.value).ToList();

            foreach (var cascadeSlot in cascadedSlots)
            {
                if (primarySlots.Count == 0)
                {
                    break; // No more available primary slots
                }

                Slot targetPrimarySlot = primarySlots[0]; // Get the first empty primary slot
                primarySlots.RemoveAt(0); // Remove the used slot from the list

                if (cascadeSlot.currentItem != null)
                {
                    // Move the item from the cascaded slot to the primary slot
                    targetPrimarySlot.PlaceItem(cascadeSlot.currentItem);

                    // Enable DragAndDropUI on the item in the primary slot
                    DragAndDropUI dragAndDropUI = cascadeSlot.currentItem.GetComponent<DragAndDropUI>();
                    if (dragAndDropUI != null)
                    {
                        dragAndDropUI.enabled = true; // Enable drag and drop
                    }

                    // Apply primary slot visuals and notify the event manager
                    targetPrimarySlot.ApplyPrimaryVisuals();
                    EventManager.Instance.ItemPlaced(targetPrimarySlot);

                    // Disable DragAndDropUI in the cascaded slot (since it's now empty)
                    dragAndDropUI = cascadeSlot.GetComponentInChildren<DragAndDropUI>();
                    if (dragAndDropUI != null)
                    {
                        dragAndDropUI.enabled = false; // Disable drag and drop
                    }
                }
            }
        }
    }


    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
    }

    public void LogShelvesState()
    {
        foreach (var shelf in shelves)
        {
            foreach (var slot in shelf.slots)
            {
                string status = slot.IsEmpty() ? "Empty" : $"Occupied by {slot.currentItem.name}";
            }
        }
    }

    private void HandleScore()
    {
        float currentTime = Time.time;

        if (currentTime - lastMatchTime <= comboDuration)
        {
            comboMultiplier += 1f;
        }
        else
        {
            comboMultiplier = 1f;
        }
        lastMatchTime = currentTime;
        score += Mathf.RoundToInt(scorePerMatch * comboMultiplier);
    }
}
