using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsRepository : MonoBehaviour
{
    public static Dictionary<ItemType, MergeableItemData> Mergeables;
    public static Dictionary<ItemType, MergeableItemData> Recipes;
    [SerializeField] private SerializedDictionary<ItemType, MergeableItemData> mergeables = new();
    [SerializeField] private SerializedDictionary<ItemType, MergeableItemData> recipes;

    private void Awake()
    {
        Mergeables = mergeables;
        Recipes = recipes;
    }
}
