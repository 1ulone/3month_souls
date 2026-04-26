using UnityEngine;

public class DamageComponent : MonoBehaviour
{
    public int damage { get; set; }
    public bool destroyOnEnd = false;

    private void Awake()
        => damage = 1; 
}
