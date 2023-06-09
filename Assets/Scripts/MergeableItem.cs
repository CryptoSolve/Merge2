using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;

public class MergeableItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public static Transform ParentPanel { get; private set; }
    public static MergeableItem SelectedItem { get; private set; }
    public static MergeableItem SecondItem { get; private set; }
    [field: SerializeField] public MergeableItemData MergeableItemData { get; private set; }
    public Action<MergeableItem, MergeableItem> OnSuccessfullMerge;
    public Vector2 ChestPosition { get; private set; }
    private static Canvas canvas;
    private static Camera myCamera;
    private static Action<int> incrementScore;
    private Vector2 homePosition;
    private Coroutine followMouseRoutine;
    private ItemType itemType;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemLevel;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image mainImage;
    [SerializeField] private Image highlight;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        OnSuccessfullMerge += OnSuccess;

        Invoke(nameof(SetHomePosition), 0.1f);
    }

    public void Init(
        Transform parentPanel,
        Canvas canvas,
        Camera camera,
        Action<int> incrementScore,
        Vector2 chestPosition,
        MergeableItemData data)
    {
        ParentPanel = parentPanel;
        ChestPosition = chestPosition;
        SetData(data);

        if (MergeableItem.canvas != null) return;
        MergeableItem.canvas = canvas;
        if (MergeableItem.myCamera != null) return;
        MergeableItem.myCamera = camera;
        if (MergeableItem.incrementScore != null) return;
        MergeableItem.incrementScore = incrementScore;
    }

    public void SetData(MergeableItemData data)
    {
        MergeableItemData = data;
        mainImage.sprite = data.Sprite;
        itemName.text = data.Name;
        itemLevel.text = data.Level.ToString();
        itemType = data.Type;
    }

    public void Select()
    {
        highlight.DOFade(1f, 0.2f);
    }

    public void Unselect()
    {
        highlight.DOFade(0f, 0.2f);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Select();
        SelectedItem = this;

        transform.SetAsLastSibling();
        canvasGroup.blocksRaycasts = false;

        followMouseRoutine = StartCoroutine(FollowMouse(0.02f));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Unselect();
        if (SelectedItem == null) return;

        canvasGroup.blocksRaycasts = true;

        if (SecondItem == null)
        {
            OnFail();
        }
        else
        {
            int itemLevel = SecondItem.MergeableItemData.Level;
            bool success = TryMerge(SelectedItem.itemType, SecondItem.itemType, SecondItem);

            if (success)
            {
                OnSuccessfullMerge?.Invoke(SelectedItem, SecondItem);
                incrementScore.Invoke(itemLevel);
            }
            else
            {
                OnFail();
            }

            SecondItem.Unselect();
            SecondItem = null;
        }

        SelectedItem = null;
        StopCoroutine(followMouseRoutine);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (SelectedItem == null) return;

        Select();
        SecondItem = this;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (SelectedItem == null) return;
        if (this == SelectedItem) return;

        Unselect();
        SecondItem = null;
    }

    private bool TryMerge(ItemType firstItem, ItemType secondItem, MergeableItem itemToUpgrade)
    {
        if (firstItem != secondItem) return false;

        MergeableItemData upgradedItem;
        if (ItemsRepository.Recipes.TryGetValue(secondItem, out upgradedItem))
        {
            itemToUpgrade.SetData(upgradedItem);
            return true;
        }
        return false;
    }

    private void Activate()
    {
        canvasGroup.interactable = true;
    }

    private void Deactivate()
    {
        canvasGroup.interactable = false;
    }

    private void OnSuccess(MergeableItem firstItem, MergeableItem secondItem)
    {
        Deactivate();
        var firstItemTransform = firstItem.transform;
        var secondItemTransform = secondItem.transform;

        DOTween.Sequence()
            .Append(secondItemTransform.DOScale(1.5f, 0.3f).SetEase(Ease.OutQuad))
            /*
             * //Shake Animation
            .Append(SecondItem.transform.DORotate(Vector3.forward * 20, 0.04f))
            .Append(SecondItem.transform.DORotate(Vector3.forward * -20, 0.08f))
            .Append(SecondItem.transform.DORotate(Vector3.forward * 20, 0.08f))
            .Append(SecondItem.transform.DORotate(Vector3.forward * -20, 0.08f))
            .Append(SecondItem.transform.DORotate(Vector3.forward * 20, 0.08f))
            .Append(SecondItem.transform.DORotate(Vector3.forward * 0, 0.04f
            */
            .Append(secondItemTransform.DOScale(1f, 0.2f).SetEase(Ease.InQuad));

        DOTween.Sequence()
            .Append(firstItemTransform.DOScale(0, 0.2f))
            .Append(firstItemTransform.DOMove(ChestPosition, 0f))
            .Append(firstItemTransform.DOScale(1, 0.3f));

        firstItemTransform.DOMove(homePosition, 0.6f).SetDelay(0.4f);

        Invoke(nameof(Activate), 0.9f);
    }

    private void OnFail()
    {
        Deactivate();

        float animationTime = 0.4f;
        transform.DOMove(homePosition, animationTime);

        Invoke(nameof(Activate), animationTime);
    }

    private void SetHomePosition()
    {
        homePosition = transform.position;
    }

    private IEnumerator FollowMouse(float timeStep)
    {
        WaitForSeconds wait = new(timeStep);

        while (true)
        {
            transform.position = Input.mousePosition;
            yield return wait;
        }
    }
}