using UnityEngine;

[CreateAssetMenu(fileName = "[EType]Data", menuName="Assets/Data/Enemy")]
public class EnemyData : ScriptableObject 
{
    public int health;
    public int damage = 2;
    public float movementSpeed;
    public float chaseSpeed;

    public float idleLength;
    public float patrolLength;
    public float cooldownLength;

    public float chaseRadius;
    public float attackRadius;

    public LayerMask floor;
    public LayerMask collider;
    public LayerMask player;
    public LayerMask hurtForEnemy;

    public bool checkChase = true;
    public bool checkPlayerElevation = true;

    public float walkRange = 5;

    [HideInInspector] public bool onGround;
    [HideInInspector] public bool onWall;
    [HideInInspector] public bool onChase;
    [HideInInspector] public bool onAttack;
}

