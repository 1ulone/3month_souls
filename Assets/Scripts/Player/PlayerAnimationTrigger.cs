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
        
        Vector3 d = controller.Pointer.position - controller.transform.position;
        float angle = Mathf.Atan2(d.z, d.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360.0f;

        attackVFX.SetActive(true);
        attackVFX.transform.position = transform.position + new Vector3(0, heightMod, 0) + (transform.forward * 1.25f);
        attackVFX.transform.rotation = Quaternion.Euler(new Vector3(0, angle, 0));
        attackVFX.GetComponent<DamageComponent>().damage = PlayerStats.instances.damage;
        // attackVFX.transform.LookAt(new Vector3(controller.Pointer.position.x, 0.5f, controller.Pointer.position.z));
    }
}
