using UnityEngine;
using UnityEngine.Rendering;

public class PaintManager : Singleton<PaintManager>{

    public Shader texturePaint;
    public Shader extendIslands;

    int prepareUVID = Shader.PropertyToID("_PrepareUV");
    int positionID = Shader.PropertyToID("_PainterPosition");
    int hardnessID = Shader.PropertyToID("_Hardness");
    int strengthID = Shader.PropertyToID("_Strength");
    int radiusID = Shader.PropertyToID("_Radius");
    int blendOpID = Shader.PropertyToID("_BlendOp");
    int colorID = Shader.PropertyToID("_PainterColor");
    int textureID = Shader.PropertyToID("_MainTex");
    int uvOffsetID = Shader.PropertyToID("_OffsetUV");
    int uvIslandsID = Shader.PropertyToID("_UVIslands");

    Material paintMaterial;
    Material extendMaterial;

    CommandBuffer command;

    public override void Awake(){
        base.Awake();
        
        paintMaterial = new Material(texturePaint);
        extendMaterial = new Material(extendIslands);
        command = new CommandBuffer();
        command.name = "CommmandBuffer - " + gameObject.name;
    }

    public void initTextures(Paintable paintable){
        RenderTexture mask = paintable.getMask();
        RenderTexture uvIslands = paintable.getUVIslands();
        RenderTexture extend = paintable.getExtend();
        RenderTexture support = paintable.getSupport();
        Renderer rend = paintable.getRenderer();

        command.SetRenderTarget(mask);
        command.SetRenderTarget(extend);
        command.SetRenderTarget(support);

        paintMaterial.SetFloat(prepareUVID, 1);
        command.SetRenderTarget(uvIslands);
        for (int i = 0; i < rend.sharedMaterials.Length; i++)
        {
            command.DrawRenderer(rend, paintMaterial, i);
        }
        Graphics.ExecuteCommandBuffer(command);
        command.Clear();
    }


    public void paint(Paintable paintable, Vector3 pos, float radius = 1f, float hardness = .5f, float strength = .5f, Color? color = null)
    {
        RenderTexture mask = paintable.getMask();
        RenderTexture uvIslands = paintable.getUVIslands();
        RenderTexture extend = paintable.getExtend();
        RenderTexture support = paintable.getSupport();
        Renderer rend = paintable.getRenderer();

        // 1. Extract the mesh to bypass camera culling
        Mesh mesh = rend is SkinnedMeshRenderer ? 
            ((SkinnedMeshRenderer)rend).sharedMesh : 
            rend.GetComponent<MeshFilter>().sharedMesh;

        // 2. Use a MaterialPropertyBlock to prevent data overwriting in loops
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        block.SetFloat(prepareUVID, 0);
        block.SetVector(positionID, pos);
        block.SetFloat(hardnessID, hardness);
        block.SetFloat(strengthID, strength);
        block.SetFloat(radiusID, radius);
        block.SetTexture(textureID, support);
        block.SetColor(colorID, color ?? Color.red);

        extendMaterial.SetFloat(uvOffsetID, paintable.extendsIslandOffset);
        extendMaterial.SetTexture(uvIslandsID, uvIslands);

        command.SetRenderTarget(mask);
        
        // 3. Draw the mesh directly using the property block
        command.DrawMesh(mesh, rend.transform.localToWorldMatrix, paintMaterial, 0, 0, block);

        command.SetRenderTarget(support);
        command.Blit(mask, support);

        command.SetRenderTarget(extend);
        command.Blit(mask, extend, extendMaterial);

        Graphics.ExecuteCommandBuffer(command);
        command.Clear();
    }
}
