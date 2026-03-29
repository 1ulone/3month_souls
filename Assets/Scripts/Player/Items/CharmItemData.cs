using UnityEngine;

[CreateAssetMenu(fileName="new Charm", menuName = "Assets/Data/Charm")]
public class CharmItemData : ItemData 
{
    public int vitalityBuff;
    public int strengthBuff;
    public int constitutionBuff;
    public int dexterityBuff;
    public int poiseBuff;
    public override itemType getType() { return itemType.charm; }
}
