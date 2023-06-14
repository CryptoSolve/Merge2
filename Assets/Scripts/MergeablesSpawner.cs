using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MergeablesSpawner : MonoBehaviour
{
    [field:SerializeField] public int Count { get; private set; }
    [field:SerializeField] public Transform ItemsParent { get; private set; }
    [field: SerializeField] public Transform ChestTransform { get; private set; }
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Camera _camera;
    private List<GameObject> spawnedObjects;
    private List<MergeableItem> spawnedItems;
    private Action<int> incrementScore;

    private void Awake()
    {
        spawnedObjects = new List<GameObject>(Count);
        spawnedItems = new List<MergeableItem>(Count);
        if (ItemsParent == null)
        {
            ItemsParent = transform;
        }
    }

    public void Init(Action<int> incrementScore)
    {
        this.incrementScore = incrementScore;
    }

    private void Start()
    {
        var firstLevelItems = FillItemTypes();
        var chestPosition = ItemsParent.InverseTransformVector(ChestTransform.position);

        var canvas = GetComponent<Canvas>();
        for (int i = 0; i < Count; i++)
        {
            var newObject = Instantiate(itemPrefab, ItemsParent);
            var item = newObject.GetComponent<MergeableItem>();

            MergeableItemData data = ItemsRepository.Mergeables[GetRandomItemType(firstLevelItems)];


            item.Init(ItemsParent, canvas, _camera, incrementScore, chestPosition, data);
            item.OnSuccessfullMerge += (oldItem, upgradedItem) => oldItem.SetData(ItemsRepository.Mergeables[GetRandomItemType(firstLevelItems)]);

            spawnedItems.Add(item);
            spawnedObjects.Add(newObject);
        }

        Invoke(nameof(DeactivateLayoutGroup), 0.05f);
    }

    private ItemType[] FillItemTypes()
    {
        return new[]
        {
            ItemType.WeaponOne,
            ItemType.SpellOne,
            ItemType.ArmorOne,
            ItemType.ElixirOne
        };
    }

    private ItemType GetRandomItemType(ItemType[] itemTypes)
    {
        System.Random random = new();

        int randomNumber = random.Next(itemTypes.Length);
        return itemTypes[randomNumber];
    }

    private void DeactivateLayoutGroup()
    {
        ItemsParent.GetComponent<LayoutGroup>().enabled = false;
    }
}