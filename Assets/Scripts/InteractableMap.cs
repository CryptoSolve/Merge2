using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableMap : MonoBehaviour, IInteractable, IHoverable
{
    public event Action OnMapActivated;
    [SerializeField] private ParticleSystem vfx;
    [SerializeField] private Transform homeTransform;
    [SerializeField] private Transform hoveredTransform;
    [SerializeField] private Transform activatedTransform;

    public bool IsHovered => isHovered;
    private bool isHovered;

    public void Interact()
    {
        print("You found a map!");
        DOTransform(activatedTransform, 1f);
        Invoke(nameof(EmitMapIsActive), 2f);
    }

    public void OnHover()
    {
        isHovered = true;
        vfx.Play();
        DOTween.Sequence()
            .Append(vfx.transform.DOScale(0, 0))
            .Append(vfx.transform.DOScale(1, 0.5f));
        DOTransform(hoveredTransform, 1f);
    }

    public void OnHoverEnd()
    {
        isHovered = false;
        Invoke(nameof(KillVFX), 0.8f);
        vfx.transform.DOScale(0, 0.5f);
        DOTransform(homeTransform, 1f);
    }

    private void KillVFX()
    {
        if (isHovered)
            return;

        vfx.Stop();
        vfx.Clear();
    }

    private void DOTransform(Transform t, float time)
    {
        transform.DOMove(t.position, time);
        transform.DORotate(t.rotation.eulerAngles, time);
    }

    private void EmitMapIsActive()
    {
        OnMapActivated?.Invoke();
    }
}