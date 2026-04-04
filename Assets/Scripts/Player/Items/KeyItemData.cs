using UnityEngine;

[CreateAssetMenu(fileName="new Key Item", menuName = "Assets/Data/KeyItem")]
public class KeyItemData : ItemData 
{
    public Sprite mainDisplay;
    public override itemType getType() { return itemType.keyItem; }
}
