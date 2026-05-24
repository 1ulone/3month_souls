using UnityEngine;
using System.Collections;

public class PlayerHitboxController : MonoBehaviour
{
    [SerializeField] private PlayerController controller;
    [SerializeField] private LayerMask parryWindow;

     private void OnTriggerEnter(Collider other)
     {
         if (other.TryGetComponent<DamageComponent>(out DamageComponent dc))
         {
             if (!dc.parryAble)
                 return;

             dc.ParryCallback(controller.transform.position, PlayerStats.instances.damage);
             StartCoroutine(parryEffect());
         }
     }

     private IEnumerator parryEffect() 
     {
         controller.canBeHurt = false;

         StartCoroutine(FadeTransitionUI.instances.FadeInOut(false, false, 0.125f, true));
         TimeManager.instances.HitStop(0.1f);
         Pool.instances.CreateObject("ParrySparks", new Vector3(transform.position.x, 0.5f, transform.position.z), new Vector3(0, 90, 0));

         yield return new WaitUntil(()=> Time.timeScale == 1);
         controller.canBeHurt = true;
     }
}
