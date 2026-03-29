using UnityEngine;
using UnityEditor;

public enum itemType
{
    weapon,
    charm,
    keyItem
}

public class ItemData : ScriptableObject 
{
    public string tag;
    public Sprite icon;

    [HideInInspector][SerializeField] public string description = "";
    public virtual itemType getType() { return itemType.weapon; } 
}

[CustomEditor(typeof(ItemData), true), CanEditMultipleObjects]
public class ItemDataEditor : Editor 
{
    private const int _maxDigit = 91;
    private ItemData _dTarget;
    private string _placeHolder = string.Empty;

    public override void OnInspectorGUI() 
    {
        _dTarget = (ItemData)target;
    
        _placeHolder = _dTarget.description;
        if(_placeHolder.Length > _maxDigit)
        {
            // remember to reassign the value back to the place
            _dTarget.description = _placeHolder.Substring(0, _maxDigit - 1);
        }

        DrawDefaultInspector();
        DrawCustomInspector();
    }

    void DrawCustomInspector() 
    {
        GUIStyle guiStyle = EditorStyles.textArea;
        guiStyle.wordWrap = true;

        EditorGUI.BeginChangeCheck();
        GUILayout.Label("Description");

        _dTarget.description = EditorGUILayout.TextArea(_dTarget.description, guiStyle, new GUILayoutOption[] 
        { 
            GUILayout.MinHeight(50f),
            GUILayout.MinWidth(350f),
        });

        if(EditorGUI.EndChangeCheck()){
            if(_dTarget.description.Length >= _maxDigit){
            _dTarget.description.Remove((_dTarget.description.Length - 1) - 3, 3);
            }
        }

        GUILayout.Space(5f);
        GUILayout.Label("Character Length : " + _dTarget.description.Length, EditorStyles.boldLabel);
    }
}
