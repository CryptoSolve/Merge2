using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MetaManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private InteractableMap map;
    [SerializeField] private Transform helper;
    private Action OnBackToMainMenu;
    private IHoverable hoverable;
    private IInteractable interactable;
    private int hoverableMask;
    private int interactableMask;
    private bool isInTransitionToCore;

    private void Awake()
    {
        playerInput.onActionTriggered += PlayerInput_onActionTriggered;
        hoverableMask = 1 << LayerMask.NameToLayer("Hoverable");
        interactableMask = 1 << LayerMask.NameToLayer("Interactable");
    }

    private void Start()
    {
        int core = (int)Scenes.Core;
        int mainMenu = (int)Scenes.MainMenu;
        map.OnMapActivated += () => SceneManager.LoadScene(core);
        OnBackToMainMenu += () => SceneManager.LoadScene(mainMenu);
    }

    private void Update()
    {
        if (isInTransitionToCore)
            return;
        if (FindItem(out IHoverable newHoverable, interactableMask))
        {
            if (hoverable != newHoverable)
            {
                hoverable?.OnHoverEnd();
                hoverable = newHoverable;
                hoverable.OnHover();
            }
        }
        else if (hoverable != null)
        {
            hoverable.OnHoverEnd();
            hoverable = null;
        }
    }

    public void BackToMainMenu()
    {
        OnBackToMainMenu?.Invoke();
    }

    public bool FindItem<T>(out T result, int layerMask)
    {
        result = default;

        var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction, Color.red, 0.5f);
        Debug.DrawLine(ray.origin, ray.direction, Color.green, 0.5f);

        if (Physics.Raycast(ray, out RaycastHit hit))
            helper.position = hit.point;

        if (!Physics.Raycast(ray, out hit, 100f, layerMask))
            return false;
        if (!hit.collider.TryGetComponent(out result))
            return false;
        return true;
    }

    private void PlayerInput_onActionTriggered(InputAction.CallbackContext ctx)
    {
        if (ctx.action.phase == InputActionPhase.Started)
        {
        }
        else if (ctx.action.phase == InputActionPhase.Canceled)
        {
            if (FindItem(out interactable, interactableMask))
            {
                isInTransitionToCore = true;
                interactable.Interact();
            }
        } 
    }

    private void OnValidate()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }
}
