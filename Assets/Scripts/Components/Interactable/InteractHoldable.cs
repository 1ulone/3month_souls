using UnityEngine;

public class InteractHoldable : MonoBehaviour, IInteractable 
{
    [SerializeField] public float health = 1;
    [SerializeField] public float throwSpeed = 5;
    [SerializeField] public bool canBeShield = false;

    private BulletComponent bulletComponent;

    public Rigidbody rb { get; set; }
    public Transform realTransform { get; set; }
    public DamageComponent damageComponent { get; set; }

    private void Start()
    {
        realTransform = transform.parent;
        bulletComponent = GetComponentInParent<BulletComponent>();
        damageComponent = GetComponent<DamageComponent>();

        rb = GetComponentInParent<Rigidbody>();

        bulletComponent.health = health;
        bulletComponent.doRotate = true;
    }

    public void Interact(Transform other = null) 
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController p))
            p.ChangeHoldItem(this);
    }

    public void Sling(Vector3 dir)
    {
        bulletComponent.Move(throwSpeed, new Vector3(dir.normalized.x, 0, dir.normalized.z), ()=> 
        {
            realTransform.gameObject.layer = 0;
            this.gameObject.layer = 9;
        }, 1.0f);
    }
}
