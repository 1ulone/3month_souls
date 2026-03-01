using System.Collections;
using UnityEngine;

public class HitflashComponent : MonoBehaviour
{
    // NOTE: since the model i downloaded is separated (meaning multiple Renderer) this is just a placeholder
    // anyway, later on i'll just have 1 Renderer
    [SerializeField] private Renderer[] renderers;

    private MaterialPropertyBlock propBlock;
    private int intensityID;
    private const float flashDelay = 0.05f;

    private void Awake()
    {
        // rend = GetComponent<Renderer>();
        propBlock = new MaterialPropertyBlock();
        intensityID = Shader.PropertyToID("_flashIntensity"); 
    }

    private void SetFlashIntensity(float intensity)
    {
        foreach(Renderer rend in renderers)
        {
            rend.GetPropertyBlock(propBlock, 1);
            propBlock.SetFloat(intensityID, intensity);
            rend.SetPropertyBlock(propBlock, 1);
        }
    }

    public IEnumerator FlashesCoroutine()
    {
        int i = 0;
        while (i < 6) 
        {
            SetFlashIntensity(1);
            yield return new WaitForSecondsRealtime(flashDelay);
            SetFlashIntensity(0);
            yield return new WaitForSecondsRealtime(flashDelay);
            i++;
        }
    }
}
