using UnityEngine;

namespace wine.util
{
    public class OnLightingEvent : MonoBehaviour
    {
        private Animator anim;

        private void Awake()
            => anim = GetComponent<Animator>();

        private void Start()
            => onEndLighting();

        private void OnDisable()
        {
            anim.enabled = false;
            CancelInvoke();
        }

        private void StartLighting()
        {
            anim.enabled = true;
        }

        private void onEndLighting()
        {
            anim.enabled = false;
            float timer = Random.Range(0, 100.0f);
            Invoke("StartLighting", timer);
        }
    }
}
