using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum invMode
{
    main,
    key,
    map
}

public class InventoryUI : MonoBehaviour 
{
    public static InventoryUI instances;
    private InputController input;
    private invMode mode;

    [SerializeField] private Image[] slotUI;
    [SerializeField] private Image[] keyslotUI;

    // [SerializeField] private slotImage[] slotUI;
    // [SerializeField] private slotImage[] keyslotUI;

/*--NOTE: Inventory Slot Variable, and a 'Reset' function for main inventory data-*/
    private slot[][] slot;
    private void ClearMainSlot() 
    {
        slot = new slot[][]
        {
            new slot[] { new slot(), new slot(), new slot(), new slot(), new slot() },
            new slot[] { new slot(), new slot(), new slot(), new slot(), new slot() },
            new slot[] { new slot(), new slot(), new slot(), new slot(), new slot() },
            new slot[] { new slot(), new slot(), new slot(), new slot(), new slot() },
            new slot[] { new slot(), new slot(), new slot(), new slot(), new slot() },
            new slot[] { new slot(), new slot(), new slot(), new slot(), new slot() }
        };

        int y = 0, x = 0;
        for (int i = 0; i < slotUI.Length; i++)
        {
            slot[x][y].image = slotUI[i];

            if ((i+1) % 5 == 0) 
                { y = 0; x++; }
            else 
                { y++; }
        }
    }

/*--NOTE: Key Inventory Slot Variable, and a 'Reset' function for key inventory data-*/
    private slot[][] keyslot; 
    private void ClearKeySlot() 
    {
        keyslot = new slot[][]
        {
            new slot[] { new slot(), new slot(), new slot(), new slot(), new slot() },
            new slot[] { new slot(), new slot(), new slot(), new slot(), new slot() },
            new slot[] { new slot(), new slot(), new slot(), new slot(), new slot() },
            new slot[] { new slot(), new slot(), new slot(), new slot(), new slot() },
            new slot[] { new slot(), new slot(), new slot(), new slot(), new slot() },
            new slot[] { new slot(), new slot(), new slot(), new slot(), new slot() },
        };
 
        int y = 0, x = 0;
        for (int i = 0; i < keyslotUI.Length; i++)
        {
            keyslot[x][y].image = keyslotUI[i];

            if ((i+1) % 5 == 0) 
                { y = 0; x++; }
            else 
                { y++; }
        }
   }

    public static bool hasKeyItem(ItemData k)
    {
        for (int x = 0; x < InventoryUI.instances.keyslot.Length; x++)
        {
            for (int y = 0; y < InventoryUI.instances.keyslot[x].Length; y++)
            {
                if (k == InventoryUI.instances.keyslot[x][y].item as KeyItemData)
                    return true;
            }
        }

        return false;
    }

    private int col, row;
    private int optionsID = 0;
    private bool isOnInventory, isOnEnumerator, isOnOptions;
    private CanvasGroup mainCanvas;

    [SerializeField] private CanvasGroup equipableCanvas;
    [SerializeField] private CanvasGroup keyitemsCanvas;
    [SerializeField] private CanvasGroup mapCanvas;

    [SerializeField] private CanvasGroup itemOptionCanvas;
    [SerializeField] private TextMeshProUGUI[] itemOptionGUI;

    [SerializeField] private RectTransform cursor;

    [SerializeField] private Image equippedWeapon;
    [SerializeField] private Image equippedCharm;

    [SerializeField] private TextMeshProUGUI health;
    [SerializeField] private TextMeshProUGUI damage;
    [SerializeField] private TextMeshProUGUI defense;
    [SerializeField] private TextMeshProUGUI speed;
    [SerializeField] private TextMeshProUGUI rollspeed;
    [SerializeField] private TextMeshProUGUI downtime;
    [SerializeField] private TextMeshProUGUI knockforce;

    [SerializeField] private Image itemDisplay;
    [SerializeField] private TextMeshProUGUI itemLabel;
    [SerializeField] private TextMeshProUGUI itemDescription;

    private Vector2 defaultCursorSize;

/* NOTE:--------------------------------------------
 *------------------INITIALIZATION------------------
 *--------------------------------------------------*/
    private void Awake()
    {
        instances = this;
        mainCanvas = GetComponent<CanvasGroup>();

        ToggleCanvasGroup(equipableCanvas, false);
        ToggleCanvasGroup(keyitemsCanvas, false);
        ToggleCanvasGroup(mapCanvas, false);
        ToggleCanvasGroup(mainCanvas, false);

        ClearMainSlot();
        ClearKeySlot();
    }

    private void Start()
    {
        input = FindFirstObjectByType<InputController>();
        defaultCursorSize = cursor.sizeDelta;
    }

    private void ToggleCanvasGroup(CanvasGroup cgt, bool b)
    {
        cgt.alpha = b ? 1 : 0;
        cgt.interactable = b;
        cgt.blocksRaycasts = b;
    }

    public void ToggleInventory()
    {
        if (isOnEnumerator)
            return;

        isOnInventory = !isOnInventory;
        Time.timeScale = isOnInventory ? 0 : 1;
        StartCoroutine(isOnInventory ? EnterInventory() : ExitInventory());
    }

    private IEnumerator EnterInventory() 
    {
        isOnEnumerator = true;
        mode = invMode.main;
        yield return new WaitForSecondsRealtime(0.15f);

        col = 0; row = 0;
        cursor.position = slot[col][row].image.transform.position;

        health.text = "Health : " + PlayerStats.instances.health.ToString();
        damage.text = "Damage : " + PlayerStats.instances.damage.ToString();
        defense.text = "Defense : " + PlayerStats.instances.defense.ToString();
        speed.text = "Speed : " + PlayerStats.instances.speed.ToString();
        rollspeed.text = "Roll Speed : " + PlayerStats.instances.rollspeed.ToString();
        downtime.text = "Downtime : " + PlayerStats.instances.downtime.ToString();
        knockforce.text = "KnockForce : " + PlayerStats.instances.knockforce.ToString();

        cursor.GetComponent<Image>().color = Color.yellow;

        ToggleCanvasGroup(mainCanvas, true);
        ToggleCanvasGroup(equipableCanvas, true);
        isOnEnumerator = false;
    }

    private IEnumerator ExitInventory() 
    {
        isOnEnumerator = true;
        yield return new WaitForSecondsRealtime(0.15f);

        ToggleCanvasGroup(equipableCanvas, false);
        ToggleCanvasGroup(keyitemsCanvas, false);
        ToggleCanvasGroup(mapCanvas, false);
        ToggleCanvasGroup(mainCanvas, false);
        isOnEnumerator = false;
    }

    private void Update()
    {
        if (isOnEnumerator)
            return;

        if (!isOnInventory)
            return;

        Vector2 rawdir = input.move.ReadValue<Vector2>();
        Vector2Int dir = new Vector2Int((int)rawdir.x, (int)rawdir.y);

        if (dir != Vector2.zero)
        {
            switch(mode)
            {
                case (invMode.main) : 
                {
                    if (!isOnOptions)
                        StartCoroutine(Move_OnEquipableItemPanel(dir)); else 
                    if (isOnOptions)
                        StartCoroutine(MoveOptions_OnEquipableItemPanel(dir));
                } break;
                case (invMode.key) :
                {
                    StartCoroutine(Move_OnKeyItemPanel(dir));
                } break;
                case (invMode.map) :
                {

                } break;
                default: { return; };
            }
        }

        if (input.switchInventoryLeft.WasPressedThisFrame())
        {
            switch(mode)
            {
                case (invMode.main) : { StartCoroutine(ChangeInventoryMode(invMode.map, equipableCanvas)); } break;
                case (invMode.key) : { StartCoroutine(ChangeInventoryMode(invMode.main, keyitemsCanvas)); } break;
                case (invMode.map) : { return; };
                default: return;
            }
        }

        if (input.switchInventoryRight.WasPressedThisFrame())
        {
            switch(mode)
            {
                case (invMode.main) : { StartCoroutine(ChangeInventoryMode(invMode.key, equipableCanvas)); } break;
                case (invMode.map) : { StartCoroutine(ChangeInventoryMode(invMode.main, mapCanvas)); } break;
                case (invMode.key) : { return; };
                default: return;
            }
        }

        if (input.interact.WasPressedThisFrame() && slot[col][row].item != null && mode == invMode.main)
        {
            if (!isOnOptions)
                StartCoroutine(OpenOption_OnEquipableItemPanel()); else 
            if (isOnOptions)
            {
                switch (optionsID)
                {
                    case 0: { if (itemOptionGUI[0].text == "Equip") { StartCoroutine(EquipItem()); } else { StartCoroutine(DequipItem()); } } break;
                    case 1: { /*Discard*/ } break;
                    case 2: { StartCoroutine(CloseOption_OnEquipableItemPanel()); } break;
                    default: { return; }
                }
            }
        }
    }

    private IEnumerator ChangeInventoryMode(invMode m, CanvasGroup prevCanvas)
    {
        isOnEnumerator = true;
        ToggleCanvasGroup(prevCanvas, false);
        // TODO: Animation here 
        yield return new WaitForSecondsRealtime(0.15f);

        switch(m)
        {
            case (invMode.main) : 
            {
                col = 0; row = 0;
                cursor.position = slot[col][row].image.transform.position;

                health.text = "Health : " + PlayerStats.instances.health.ToString();
                damage.text = "Damage : " + PlayerStats.instances.damage.ToString();
                defense.text = "Defense : " + PlayerStats.instances.defense.ToString();
                speed.text = "Speed : " + PlayerStats.instances.speed.ToString();
                rollspeed.text = "Roll Speed : " + PlayerStats.instances.rollspeed.ToString();
                downtime.text = "Downtime : " + PlayerStats.instances.downtime.ToString();
                knockforce.text = "KnockForce : " + PlayerStats.instances.knockforce.ToString();

                cursor.GetComponent<Image>().color = Color.yellow;

                ToggleCanvasGroup(equipableCanvas, true);
            } break;
            case (invMode.key) : 
            {
                col = 0; row = 0;
                cursor.position = keyslot[col][row].image.transform.position;
                cursor.GetComponent<Image>().color = Color.yellow;

                if (keyslot[col][row].item != null)
                {
                    KeyItemData item = keyslot[col][row].item as KeyItemData;
                    itemDisplay.sprite = item.mainDisplay;
                    itemDisplay.enabled = true;
                    itemLabel.text = item.tag;
                    itemDescription.text = item.description;
                }
                else 
                {
                    itemDisplay.enabled = false;
                    itemLabel.text = "";
                    itemDescription.text = "";
                }

                ToggleCanvasGroup(keyitemsCanvas, true);
            }  break;
            case (invMode.map) : { ToggleCanvasGroup(mapCanvas, true); } break;
            default: { yield return null; } break;
        }
        mode = m;
        Debug.Log(m);
        isOnEnumerator = false;
    }

    private IEnumerator Move_OnEquipableItemPanel(Vector2Int dir)
    {
        isOnEnumerator = true;
        if (dir.y != 0)
            col -= dir.y;
        if (dir.x != 0)
            row += dir.x;

        if (col > slot.Length-1) 
            col = 0; 
        if (col < 0)
            col = slot.Length-1;

        if (row > slot[col].Length-1) 
            row = 0; 
        if (row < 0)
            row = slot[col].Length-1;

        // Debug.Log(dir);
        // Debug.Log(col + "," + row);

        yield return new WaitForSecondsRealtime(0.15f);
        cursor.position = slot[col][row].image.transform.position;
        isOnEnumerator = false;
    }

    private IEnumerator OpenOption_OnEquipableItemPanel()
    {
        isOnOptions = true;
        isOnEnumerator = true;
        optionsID = 0;
        itemOptionCanvas.transform.position = slot[col][row].image.transform.position;
        cursor.position = itemOptionGUI[0].transform.position;
        cursor.sizeDelta = new Vector2(itemOptionGUI[0].preferredWidth, itemOptionGUI[0].preferredHeight);

        // TODO: Animation here later
        yield return new WaitForSecondsRealtime(0.15f);
        ToggleCanvasGroup(itemOptionCanvas, true);
        cursor.GetComponent<Image>().color = Color.red;

        ItemData i = slot[col][row].item;

        if (i.getType() == itemType.weapon)
        {
            WeaponItemData w = i as WeaponItemData;
            if (!PlayerStats.instances.hasNoWeapon())
                itemOptionGUI[0].text = "Dequip"; 
            else 
                itemOptionGUI[0].text = "Equip"; 

            // TODO: fix later but the concept is something around like this;
            // health.text = "Health : " + PlayerStats.instances.previewHealth(w.vitalityBuff, w.constitutionBuff, w.poiseBuff).ToString();
            // damage.text = "Damage : " + PlayerStats.instances.previewDamage(w.vitalityBuff, w.dexterityBuff).ToString();
            // defense.text = "Defense : " + PlayerStats.instances.previewDefense(w.constitutionBuff, w.poiseBuff).ToString();
            // speed.text = "Speed : " + PlayerStats.instances.previewSpeed(w.dexterityBuff).ToString();
            // rollspeed.text = "Roll Speed : " + PlayerStats.instances.previewRollspeed(w.dexterityBuff, w.constitutionBuff).ToString();
            // downtime.text = "Downtime : " + PlayerStats.instances.previewDowntime(w.poiseBuff, w.dexterityBuff).ToString();
            // knockforce.text = "KnockForce : " + PlayerStats.instances.previewKnockforce(w.poiseBuff, w.vitalityBuff, w.dexterityBuff).ToString();
        } else 
        if (i.getType() == itemType.charm)
        {
            CharmItemData w = i as CharmItemData;
            if (!PlayerStats.instances.hasNoCharm())
                itemOptionGUI[0].text = "Dequip"; 
            else 
                itemOptionGUI[0].text = "Equip"; 

            // TODO: fix later but the concept is something around like this;
            // health.text = "Health : " + PlayerStats.instances.previewHealth(w.vitalityBuff, w.constitutionBuff, w.poiseBuff).ToString();
            // damage.text = "Damage : " + PlayerStats.instances.previewDamage(w.vitalityBuff, w.dexterityBuff).ToString();
            // defense.text = "Defense : " + PlayerStats.instances.previewDefense(w.constitutionBuff, w.poiseBuff).ToString();
            // speed.text = "Speed : " + PlayerStats.instances.previewSpeed(w.dexterityBuff).ToString();
            // rollspeed.text = "Roll Speed : " + PlayerStats.instances.previewRollspeed(w.dexterityBuff, w.constitutionBuff).ToString();
            // downtime.text = "Downtime : " + PlayerStats.instances.previewDowntime(w.poiseBuff, w.dexterityBuff).ToString();
            // knockforce.text = "KnockForce : " + PlayerStats.instances.previewKnockforce(w.poiseBuff, w.vitalityBuff, w.dexterityBuff).ToString();
        }

        isOnEnumerator = false;
    }

    private IEnumerator CloseOption_OnEquipableItemPanel()
    {
        isOnEnumerator = true;
        ToggleCanvasGroup(itemOptionCanvas, false);
        cursor.position = slot[col][row].image.transform.position;
        cursor.sizeDelta = defaultCursorSize;
        cursor.GetComponent<Image>().color = Color.yellow;

        // TODO: Animation here later
        yield return new WaitForSecondsRealtime(0.15f);

        health.text = "Health : " + PlayerStats.instances.health.ToString();
        damage.text = "Damage : " + PlayerStats.instances.damage.ToString();
        defense.text = "Defense : " + PlayerStats.instances.defense.ToString();
        speed.text = "Speed : " + PlayerStats.instances.speed.ToString();
        rollspeed.text = "Roll Speed : " + PlayerStats.instances.rollspeed.ToString();
        downtime.text = "Downtime : " + PlayerStats.instances.downtime.ToString();
        knockforce.text = "KnockForce : " + PlayerStats.instances.knockforce.ToString();

        isOnEnumerator = false;
        isOnOptions = false;
    }

    private IEnumerator MoveOptions_OnEquipableItemPanel(Vector2Int dir)
    {
        isOnEnumerator = true;
        if (dir.y != 0)
            optionsID -= dir.y;

        if (optionsID > itemOptionGUI.Length-1)
            optionsID = 0;
        if (optionsID < 0)
            optionsID = itemOptionGUI.Length-1;

        yield return new WaitForSecondsRealtime(0.15f);
        cursor.position = itemOptionGUI[optionsID].transform.position;
        isOnEnumerator = false;
    }

    private IEnumerator EquipItem()
    {
        isOnEnumerator = true;
        ItemData item = slot[col][row].item;
        if (item.getType() == itemType.weapon)
        {
            PlayerStats.instances.EquipWeapon(item as WeaponItemData);
            equippedWeapon.sprite = item.icon;
            equippedWeapon.enabled = true;
        } else 
        if (item.getType() == itemType.charm)
        {
            PlayerStats.instances.EquipCharm(item as CharmItemData);
            equippedCharm.sprite = item.icon;
            equippedCharm.enabled = true;
        }

        health.text = "Health : " + PlayerStats.instances.health.ToString();
        damage.text = "Damage : " + PlayerStats.instances.damage.ToString();
        defense.text = "Defense : " + PlayerStats.instances.defense.ToString();
        speed.text = "Speed : " + PlayerStats.instances.speed.ToString();
        rollspeed.text = "Roll Speed : " + PlayerStats.instances.rollspeed.ToString();
        downtime.text = "Downtime : " + PlayerStats.instances.downtime.ToString();
        knockforce.text = "KnockForce : " + PlayerStats.instances.knockforce.ToString();


        yield return new WaitForSecondsRealtime(0.15f);
        yield return CloseOption_OnEquipableItemPanel();
    }

    private IEnumerator DequipItem()
    {
        isOnEnumerator = true;
        ItemData item = slot[col][row].item;
        if (item.getType() == itemType.weapon)
        {
            PlayerStats.instances.EquipWeapon(null);
            equippedWeapon.enabled = false;
        } else 
        if (item.getType() == itemType.charm)
        {
            PlayerStats.instances.EquipCharm(null);
            equippedCharm.enabled = false;
        }

        health.text = "Health : " + PlayerStats.instances.health.ToString();
        damage.text = "Damage : " + PlayerStats.instances.damage.ToString();
        defense.text = "Defense : " + PlayerStats.instances.defense.ToString();
        speed.text = "Speed : " + PlayerStats.instances.speed.ToString();
        rollspeed.text = "Roll Speed : " + PlayerStats.instances.rollspeed.ToString();
        downtime.text = "Downtime : " + PlayerStats.instances.downtime.ToString();
        knockforce.text = "KnockForce : " + PlayerStats.instances.knockforce.ToString();

        yield return new WaitForSecondsRealtime(0.15f);
        yield return CloseOption_OnEquipableItemPanel();
    }

    private IEnumerator Move_OnKeyItemPanel(Vector2Int dir)  
    {
        isOnEnumerator = true;
        if (dir.y != 0)
            col -= dir.y;
        if (dir.x != 0)
            row += dir.x;

        if (col > slot.Length-1) 
            col = 0; 
        if (col < 0)
            col = slot.Length-1;

        if (row > slot[col].Length-1) 
            row = 0; 
        if (row < 0)
            row = slot[col].Length-1;

        // Debug.Log(dir);
        // Debug.Log(col + "," + row);

        yield return new WaitForSecondsRealtime(0.15f);
        cursor.position = keyslot[col][row].image.transform.position;
        
        if (keyslot[col][row].item != null)
        {
            KeyItemData item = keyslot[col][row].item as KeyItemData;
            itemDisplay.sprite = item.mainDisplay;
            itemDisplay.enabled = true;
            itemLabel.text = item.tag;
            itemDescription.text = item.description;
        }
        else 
        {
            itemDisplay.enabled = false;
            itemLabel.text = "";
            itemDescription.text = "";
        }

        isOnEnumerator = false;
    }

    public void AddItem(ItemData data)
    {
        if (data.getType() == itemType.keyItem) 
        {
            for (int x = 0; x < keyslot.Length; x++)
            {
                for (int y = 0; y < keyslot[x].Length; y++)
                {
                    if (keyslot[x][y].item == null)
                    {
                        keyslot[x][y].item = data;
                        keyslot[x][y].image.sprite = data.icon;

                        return;
                    }
                }
            }
        } 
        else 
        {
            for (int x = 0; x < slot.Length; x++)
            {
                for (int y = 0; y < slot[x].Length; y++)
                {
                    if (slot[x][y].item == null)
                    {
                        slot[x][y].item = data;
                        slot[x][y].image.sprite = data.icon;

                        return;
                    }
                }
            }
        }
    }
}

public class slot
{
    public Image image;
    public ItemData item;
    public TextMeshProUGUI text;//for key inventory;
}

[System.Serializable]
public class slotImage 
{ 
    [SerializeField] public Image[] image; 
}

[System.Serializable]
public class SavedInventory
{
    public List<SavedSlot> saveDataMain = new List<SavedSlot>();
    public List<SavedSlot> saveDataCore = new List<SavedSlot>();
    public List<string> saveDataRoom = new List<string>();

    public SavedInventory() {}
}

[System.Serializable]
public class SavedSlot
{
    public string itemTag;
    public string itemType;
    public int index_x;
    public int index_y;

    public SavedSlot(string tag, string type, int xx, int yy)
    {
        itemTag = tag;
        itemType = type;
        index_x = xx;
        index_y = yy;
    }
}
