using UnityEngine;


// public enum weaponType
// {
//     fist,
//     sword,
//     spear,
//     heavy,
// }

[CreateAssetMenu(fileName="new Weapon", menuName = "Assets/Data/Weapon")]
public class WeaponItemData : ItemData 
{
    public int vitalityBuff;
    public int strengthBuff;
    public int constitutionBuff;
    public int dexterityBuff;
    public int poiseBuff;
    // public AnimationClip clip;
    // public weaponType type;
    public override itemType getType() { return itemType.weapon; }
}
