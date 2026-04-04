using System.Collections.Generic;
using UnityEngine;

public class FrustumComponent : MonoBehaviour 
{
    private Renderer[] renderers;
    private List<MonoBehaviour> components = new List<MonoBehaviour>();

    private void Awake()
    {
        // Get ALL renderers in children
        renderers = GetComponentsInChildren<Renderer>(true);

        // Cache all scripts except this
        MonoBehaviour[] scomp = GetComponentsInChildren<MonoBehaviour>(true);
        foreach (MonoBehaviour c in scomp)
        {
            if (c != this)
                components.Add(c);
        }
    }

    private void LateUpdate()
    {
        if (Time.timeScale == 0)
            return;

        var planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);

        bool isVisible = false;

        // Check ALL renderer bounds
        foreach (var r in renderers)
        {
            if (GeometryUtility.TestPlanesAABB(planes, r.bounds))
            {
                isVisible = true;
                break;
            }
        }

        // Toggle renderers
        foreach (var r in renderers)
            r.enabled = isVisible;

        // Toggle scripts
        foreach (var m in components)
            m.enabled = isVisible;
    }
}
