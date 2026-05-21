using UnityEngine;

public class PlayerAnimationTrigger : MonoBehaviour
{
    [SerializeField] private PlayerController controller;
    [SerializeField] private GameObject attackVFX;
    [SerializeField] private float heightMod;

    public void TriggerHitbox()
    {
        // GameObject vfx = Pool.instances.CreateObject(
        //     "playerAttack",
        //     Vector3.zero
        // );
        
        attackVFX.SetActive(true);
        attackVFX.transform.position = transform.position + new Vector3(0, heightMod, 0) + (transform.forward * 1.25f);
        attackVFX.transform.LookAt(new Vector3(controller.Pointer.position.x, 0.5f, controller.Pointer.position.z));
        attackVFX.GetComponent<DamageComponent>().damage = PlayerStats.instances.damage;
    }
}
