using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    //Player Default Weapon (fist)
    [SerializeField] public WeaponItemData defaultWeapon;
    [SerializeField] public CharmItemData defaultCharm;
    // [SerializeField] public SkillItemData defaultSkill;

    //Level
    private const int startExp = 50;
    public int money { get; private set; }
    public int nextRequiredMoney { get; private set; }
    public int level { get; private set; }

    //Default stats on Start 
    private const int startStrength = 2;
    private const int startConstitution = 2;
    private const int startDexterity = 4;
    private const int startVitality = 6;
    private const int startPoise = 1;

    //Default stats by levels
    private int currentStrength;
    private int currentConstitution;
    private int currentDexterity;  
    private int currentVitality;
    private int currentPoise; 

    //Applicable Stats 
    public int strength { get; private set; }
    public int constitution { get; private set; }
    public int dexterity { get; private set; } 
    public int vitality { get; private set; }
    public int poise { get; private set; }

    //WeaponBuff
    private Buff vitalityBuff;
    private Buff strengthBuff;
    private Buff constitutionBuff;
    private Buff dexterityBuff;
    private Buff poiseBuff;

    public static WeaponItemData weapon;
    public static CharmItemData charm;
    // public static SkillItemData skill;

    public static int Health;
    public static int Mana;
    public static int Money;

    private void Awake()
    {
        level = 1;

        currentVitality = startVitality;
        currentStrength = startStrength;
        currentConstitution = startConstitution;
        currentDexterity = startDexterity;
        currentPoise = startPoise;

        weapon = defaultWeapon;
        charm = defaultCharm;
        // skill = defaultSkill;
        nextRequiredMoney = expFormula(level+1);
        money = 1000;

        vitalityBuff = new Buff();
        strengthBuff = new Buff();
        constitutionBuff = new Buff();
        dexterityBuff = new Buff();
        poiseBuff = new Buff();

        UpdateStat();
        Debug.Log(
            "VIT : " + vitality + "\n" +
            "ATK : " + strength + "\n" +
            "DEF : " + defense + "\n" +
            "SPD : " + speed + "\n" +
            "RSPD: " + rollspeed + "\n" +
            "DT  : " + downtime + "\n" + 
            "KB  : " + knockforce + "\n" 
        );
    }

    private void Start()
    {
        Health = health;
    }

    public bool canLevelUp(int l)
    {
        return money >= expFormula(l);
    }

    public void levelUp(int l, int atk, int con, int dex, int vit, int poi)
    {
        int cost = expFormula(l);
        level = l;
        nextRequiredMoney = expFormula(l+1);

        currentStrength = atk;
        currentConstitution = con;
        currentDexterity = dex;
        currentVitality = vit;
        currentPoise = poi;

        money -= cost;
        UpdateStat();
    }

    //----FORMULAS----//
    public int expFormula(int l)
    {
        double newExp = startExp * Math.Pow(1.3, l);
        return Mathf.RoundToInt((float)newExp);
    }

    public int basicFormula(double a, double b = 0, double c = 0)
    {
        //to make b and c returns 1 when result is calculated
        if (b == 0) { b = 2; }
        if (c == 0) { c = 4; }

        //Threshold to lower growth rate on later level
        double aThreshold = a switch { >25=>25+(a/2) ,_=> a };
        double bThreshold = b switch { >25=>25+(b/2) ,_=> b };
        double cThreshold = c switch { >25=>25+(c/2) ,_=> c };

        //Final result of all calculation;
        double result = aThreshold + (bThreshold/2) + (cThreshold/4);
        return Mathf.RoundToInt((float)result);
    }

    public float advanceFormula(double basis, double mod, double a, double b = 0)
    {
        double c = a + b;
        return Convert.ToSingle(basis + (0.05f * (Mathf.Log((float)c, 2) - mod)));
    }

    // NOTE:
    //----STATS FORMULAS----//
    public int healthFormula(int vit, int poi, int con) { return basicFormula(vit, con*2, poi*2); }
    public int damageFormula(int atk, int dex) { return basicFormula(atk, dex); }
    public int defenseFormula(int con, int poi) { return basicFormula(con, poi); }
    public int speedFormula(int dex) { return basicFormula(dex); }
    public float rollspeedFormula(int dex, int con) { return advanceFormula(2, 3, dex*0.75, con*0.25); }
    public float downtimeFormula(int poi, int dex) { return basicFormula(poi) - advanceFormula(1.5, 4, poi*0.85, dex*0.15); }
    public float knockforceFormula(int poi, int vit, int dex) { return basicFormula(vit, dex) - advanceFormula(1.25, 3.5, poi*0.45, vit*0.55); }

    // NOTE:
    //----STATS SHOWED TO PLAYER----//
    public int health { get { return healthFormula(vitality, constitution, poise); } } 
    public int damage { get { return damageFormula(strength, dexterity); } }
    public int defense { get { return defenseFormula(constitution, poise); } }
    public int speed { get { return speedFormula(dexterity); } }
    public float rollspeed { get { return rollspeedFormula(dexterity, constitution); } }
    public float downtime { get { return downtimeFormula(poise, dexterity); } }
    public float knockforce { get { return knockforceFormula(poise, vitality, dexterity); } }
    
    // NOTE:
    //----STATS PREVIEW----//
    public int previewHealth(int vit, int poi, int con)
    {
        int nVit = currentVitality + vitalityBuff.totalBuff + vit;
        int nIns = currentPoise + poiseBuff.totalBuff + poi;
        int nCon = currentConstitution + constitutionBuff.totalBuff + con;
        return healthFormula(nVit, nIns, nCon);
    }

    public int previewStrength(int atk, int dex)
    {
        int nAtk = currentStrength + strengthBuff.totalBuff + atk;
        int nDex = currentDexterity + dexterityBuff.totalBuff + dex;
        return damageFormula(nAtk, nDex);
    }

    public int previewDefense(int con, int poi)
    {
        int nCon = currentConstitution + constitutionBuff.totalBuff + con;
        int nPoi = currentPoise + poiseBuff.totalBuff + poi;
        return defenseFormula(nCon, nPoi);
    }

    public int previewSpeed(int dex)
    {
        int nDex = currentDexterity + dexterityBuff.totalBuff + dex;
        return speedFormula(nDex); 
    }

    public float previewRollspeed(int dex, int con)
    {
        int nDex = currentDexterity + dexterityBuff.totalBuff + dex;
        int nCon = currentConstitution + constitutionBuff.totalBuff + con;
        return rollspeedFormula(dex, con);
    }

    public float previewDowntime(int poi, int dex)
    {
        int nPoi = currentPoise + poiseBuff.totalBuff + poi;
        int nDex = currentDexterity + dexterityBuff.totalBuff + dex;
        return downtimeFormula(poi, dex);
    }

    public float previewKnockforce(int poi, int vit, int dex)
    {
        int nPoi = currentPoise + poiseBuff.totalBuff + poi;
        int nVit = currentVitality + vitalityBuff.totalBuff + vit;
        int nDex = currentDexterity + dexterityBuff.totalBuff + dex;
        return knockforceFormula(poi, vit, dex);
    }

    // NOTE:
    // ----EQUIP ITEM----//
    public void EquipWeapon(WeaponItemData w)
    {
        if (w == null) { w = defaultWeapon; }
        if (w != defaultWeapon)
        {
            UpdateBuff
            (
                w.vitalityBuff,
                w.strengthBuff,
                w.constitutionBuff,
                w.dexterityBuff,
                w.poiseBuff
            );
        } else 
        if (w == defaultWeapon)
        {
            UpdateBuff(0,0,0,0,0);
        }
        weapon = w;
    }

    public void EquipCharm(CharmItemData c)
    {
        if (c == null) { c = defaultCharm; }
        if (c != defaultCharm)
        {
            UpdateBuff
            (
                c.vitalityBuff,
                c.strengthBuff,
                c.constitutionBuff,
                c.dexterityBuff,
                c.poiseBuff,
                true
            );
        } else 
        if (c == defaultCharm)
        {
            UpdateBuff(0,0,0,0,0,true);
        }
        charm = c;
    }

    public void UpdateBuff(int vit, int atk, int con, int dex, int poi, bool isCharm = false)
    {
        if (!isCharm)
        {
            vitalityBuff.weapon = vit;
            strengthBuff.weapon = atk;
            constitutionBuff.weapon = con;
            dexterityBuff.weapon = dex;
            poiseBuff.weapon = poi;

            defaultWeapon.vitalityBuff = -vit;
            defaultWeapon.strengthBuff = -atk;
            defaultWeapon.constitutionBuff = -con;
            defaultWeapon.dexterityBuff = -dex;
            defaultWeapon.poiseBuff = -poi;

        } else 
        if (isCharm)
        {
            vitalityBuff.charm = vit;
            strengthBuff.charm = atk;
            constitutionBuff.charm = con;
            dexterityBuff.charm = dex;
            poiseBuff.charm = poi;

            defaultCharm.vitalityBuff = -vit;
            defaultCharm.strengthBuff = -atk;
            defaultCharm.constitutionBuff = -con;
            defaultCharm.dexterityBuff = -dex;
            defaultCharm.poiseBuff = -poi;
        }

        UpdateStat();	
    }

    public void UpdateStat()
    {
        vitality = currentVitality + vitalityBuff.totalBuff;
        strength = currentStrength + strengthBuff.totalBuff;
        constitution = currentConstitution + constitutionBuff.totalBuff;
        dexterity = currentDexterity + dexterityBuff.totalBuff;
        poise = currentPoise + poiseBuff.totalBuff;
    }

    // NOTE:
    //---------------SAVE SYSTEM-----------------//
    public object Capture() 
    {
        StatSaveData data = new StatSaveData
        (
            level,
            money,
            strength,
            constitution,
            dexterity,
            vitality,
            poise,
            weapon.tag,
            "skill",
            charm.tag,
            this.transform.position
        );	

        return data;
    }	

    // public void Restore(object state)
    // {
    //     var sd = SaveLoadManager.instances.Deserialize(state);
    //
    //     level = Convert.ToInt32(sd["level"]);
    //     money = Convert.ToInt32(sd["money"]);
    //
    //     currentStrength = Convert.ToInt32(sd["strength"]);
    //     currentConstitution = Convert.ToInt32(sd["constitution"]);
    //     currentDexterity = Convert.ToInt32(sd["dexterity"]);
    //     currentVitality = Convert.ToInt32(sd["vitality"]);
    //     currentPoise = Convert.ToInt32(sd["poise"]);
    //
    //     WeaponItemData weapon = Resources.Load<WeaponItemData>($"UI/Item Data/Data/{(string)sd["currentWeapon"]}");
    //     //SkillItemData skill = Resources.Load<SkillItemData>($"UI/Item Data/Data/{(string)sd["currentCharm"]}");
    //     CharmItemData charm = Resources.Load<CharmItemData>($"UI/Item Data/Data/{(string)sd["currentCharm"]}");
    //     Vector2 npos = new Vector2((float)Convert.ToInt32(sd["posx"]) - 4f, (float)Convert.ToInt32(sd["posy"]));
    //
    //     EquipWeapon(weapon);
    //     EquipCharm(charm);
    //     transform.position = npos;
    //
    //     UpdateStat();
    // }
}

public class Buff 
{
    public int weapon;
    public int charm;

    public int totalBuff { get { return weapon + charm; } }
}

[System.Serializable]
public class StatSaveData 
{
    public int level;
    public int money;

    public int strength;
    public int constitution;
    public int dexterity;
    public int vitality;
    public int poise;

    public string currentWeapon;
    public string currentCharm;

    public float posx;
    public float posy;

    public StatSaveData(int lvl, int m, int atk, int con, int dex, int vit, int poi, string cw, string cs, string cc, Vector2 lpos)
    {
        level = lvl;
        strength = atk;
        money = m;
        constitution = con;
        dexterity = dex;
        vitality = vit;
        poise = poi;
        currentWeapon = cw;
        currentCharm = cc;
        posx = lpos.x;
        posy = lpos.y;
    }
}
