using UnityEngine;

[CreateAssetMenu(fileName ="NewMergeableItem", menuName = "Mergeables/StandartMergeable")]
public class MergeableItemData : ScriptableObject
{
    public string Name;
    public ItemType Type;
    public int Level;
    public Sprite Sprite;
}
