using UnityEngine;

public interface IHoverable
{
    public bool IsHovered { get; }
    public void OnHover();
    public void OnHoverEnd();
}