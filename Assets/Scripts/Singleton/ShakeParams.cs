using UnityEngine;
using CameraShake;

public class ShakeParams : MonoBehaviour
{
    public static ShakeParams instances;

    [SerializeField] private PerlinShake.Params hurtSShake;
    public PerlinShake.Params HurtSShake { get { return hurtSShake; } }

    private void Awake()
        => instances = this;
}
