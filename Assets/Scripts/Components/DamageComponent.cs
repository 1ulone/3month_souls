using UnityEngine;

public class DamageComponent : MonoBehaviour
{
    public int damage { get; set; }

    private void Awake()
        => damage = 1; 
}
