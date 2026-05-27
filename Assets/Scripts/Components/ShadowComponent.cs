using UnityEngine;
using System.Collections.Generic;

public class ShadowComponent : MonoBehaviour
{
    [SerializeField] private Renderer[] renderers;
    [SerializeField] private float floorHeight;
    [SerializeField] private float lightDistance = 10;
    [SerializeField] private string lightTag = "Lightsource";

    private List<Material> shadowMat;
    private Transform closestLight = null;
    private float dist;

    private int lightDirID = Shader.PropertyToID("_LightDir");
    private int floorHeightID = Shader.PropertyToID("_FloorHeight");
    private int shadowOpacityID = Shader.PropertyToID("_OpacityMultiplier");
    
    void Start()
    {
        shadowMat = new List<Material>();
            
        foreach(Renderer r in renderers)
        {
            r.materials[1].SetFloat(floorHeightID, floorHeight);
            shadowMat.Add(r.materials[1]);
        }
    }

    void Update()
    {
        if (renderers.Length == 0)
            return;
        
        Collider[] hitLight = Physics.OverlapSphere(transform.position, lightDistance);
        dist = Vector3.Distance(transform.position, hitLight[0].transform.position);

        foreach(Collider hl in hitLight)
        {
            if (hl.CompareTag(lightTag))
            {
                float ndist = Vector3.Distance(transform.position, hl.transform.position);
                if (ndist < dist)
                {
                    dist = ndist;
                    closestLight = hl.transform;
                }
            }
        }

        if (closestLight == null)
            return;
        
        Vector3 dirToPlayer = transform.position - closestLight.position;
        foreach(Material sm in shadowMat)
        {
            float fadeMultiplier = 1.0f - Mathf.Clamp01(dist / lightDistance);

            sm.SetVector(lightDirID, dirToPlayer.normalized);
            sm.SetFloat(shadowOpacityID, fadeMultiplier);
        }
    }
}


    // [Header("Setup")]
    // public Renderer playerRenderer;
    // public Transform[] lightPoints; // Drag your torch GameObjects here in the inspector
    // public float floorHeight = 0f; // Set this to the Y level of your floor
    //
    // private Material shadowMaterial;
    // private int lightDirID = Shader.PropertyToID("_LightDir");
    // private int floorHeightID = Shader.PropertyToID("_FloorHeight");
    //
    // void Start()
    // {
    //     // Assuming the shadow material is the SECOND material in the renderer's array (Index 1)
    //     if (playerRenderer.materials.Length > 1)
    //     {
    //         shadowMaterial = playerRenderer.materials[1];
    //         shadowMaterial.SetFloat(floorHeightID, floorHeight);
    //     }
    //     else
    //     {
    //         Debug.LogError("Please add the PlanarShadow material to the player's material array!");
    //     }
    // }
    //
    // void Update()
    // {
    //     if (shadowMaterial == null || lightPoints.Length == 0) return;
    //
    //     // 1. Find the closest torch
    //     Transform closestTorch = lightPoints[0];
    //     float closestDist = Vector3.Distance(transform.position, closestTorch.position);
    //
    //     for (int i = 1; i < lightPoints.Length; i++)
    //     {
    //         float dist = Vector3.Distance(transform.position, lightPoints[i].position);
    //         if (dist < closestDist)
    //         {
    //             closestDist = dist;
    //             closestTorch = lightPoints[i];
    //         }
    //     }
    //
    //     // 2. Calculate the direction FROM the torch TO the player
    //     Vector3 directionToPlayer = transform.position - closestTorch.position;
    //
    //     // 3. Send the direction to the shader to cast the shadow accurately
    //     shadowMaterial.SetVector(lightDirID, directionToPlayer.normalized);
    // }
