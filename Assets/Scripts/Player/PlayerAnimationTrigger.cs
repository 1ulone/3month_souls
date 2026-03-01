using UnityEngine;

public class PlayerAnimationTrigger : MonoBehaviour
{
    [SerializeField] private PlayerController controller;
    [SerializeField] private float heightMod;

    public void TriggerHitbox()
    {
        GameObject vfx = Pool.instances.CreateObject(
            "playerAttack",
            transform.position + new Vector3(0, heightMod, 0) + (transform.forward * 1.25f),
            Vector3.zero
        );
        vfx.transform.LookAt(controller.Pointer);
        vfx.GetComponent<DamageComponent>().damage = 2;
        // TODO: i'll think about damage later for now lets get the feeling first
    }
}
