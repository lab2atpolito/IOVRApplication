using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class TexturePainting : MonoBehaviour
{
    [SerializeField]
    private Camera _mainCamera;

    public Texture baseTexture;
    public Material material;
    public GameObject meshGameObject;

    public Shader UVShader;
    //public Shader islandMarkerShader;
    //public Shader fixIslandEdgesShader;

    public Mesh meshToDraw;

    public static Vector3 brushWorldPosition;
    [SerializeField]
    private Brush _brush;
    [SerializeField, Range(0,10)]
    private float _brushSize = 1f;
    [SerializeField]
    private Color _brushColor;
    [SerializeField, Range(0,1)]
    private float _brushOpacity;
    [SerializeField, Range(0,1)]
    private float _brushHardness;

    // ------------------------

    //private int _clearTexture;

    //private RenderTexture _markedIslands;
    //private CommandBuffer _cbMarkingIslands;
    //private int numberOfFrames;
    //private Material fixEdgeMaterial;

    // ------------------------

    private PaintableTexture albedo;

    void Start()
    {
        // Texture and Materials initialization
        //_markedIslands = new RenderTexture(baseTexture.width, baseTexture.height, 0, RenderTextureFormat.R8);
        albedo = new PaintableTexture(Color.white, baseTexture.width, baseTexture.height, "_MainTex", UVShader, meshToDraw);
        material.SetTexture(albedo.id, albedo.runTimeTexture);


        // Command buffer inialzation
        //_cbMarkingIslands = new CommandBuffer();
        //_cbMarkingIslands.name = "markingIslands";
        //_cbMarkingIslands.SetRenderTarget(_markedIslands);
        //Material mIslandMarker = new Material(islandMarkerShader);
        //_cbMarkingIslands.DrawMesh(meshToDraw, Matrix4x4.identity, mIslandMarker);
        //_mainCamera.AddCommandBuffer(CameraEvent.AfterDepthTexture, _cbMarkingIslands);

        albedo.SetActiveTexture(_mainCamera);
    }

    private void Update()
    {
        /*if (numberOfFrames > 2)
        {
            _mainCamera.RemoveCommandBuffer(CameraEvent.AfterDepthTexture, _cbMarkingIslands);
        }
        numberOfFrames++;*/

        albedo.UpdateShaderParameters(meshGameObject.transform.localToWorldMatrix);

        // Brush World Position
        /*Vector4 brushPosition = Vector3.positiveInfinity;
        brushPosition = _brush.GetBrushingPosition();
        // brush position = contact point between brush collider and paintable object
        brushPosition.w = _brush.IsPainting() ? 1 : 0;
        brushWorldPosition = brushPosition;*/

        RaycastHit hit;
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        Vector4 mwp = Vector3.positiveInfinity;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.tag == "PaintableObject")
            {
                mwp = hit.point;
                Debug.Log("Collision");
            }
        }

        mwp.w = Input.GetMouseButton(0) ? 1 : 0;

        brushWorldPosition = mwp;
        Shader.SetGlobalVector("_BrushPosition", mwp);
        //Shader.SetGlobalVector("_BrushPosition", brushPosition);
        Shader.SetGlobalColor("_BrushColor", _brushColor);
        Shader.SetGlobalFloat("_BrushOpacity", _brushOpacity);
        Shader.SetGlobalFloat("_BrushSize", _brushSize);
        Shader.SetGlobalFloat("_BrushHardness", _brushHardness);
    }

    // HELPER FUNCTIONS
    public void SetAlbedoActive()
    {
        albedo.SetActiveTexture(_mainCamera);
    }
}

[System.Serializable]
public class PaintableTexture
{
    public string id;
    public RenderTexture runTimeTexture;
    public RenderTexture paintedTexture;

    public CommandBuffer commandBuffer;

    //private RenderTexture fixedIslands;

    private Material mPaintInUV;
    //private Material mFixedEdges;

    public PaintableTexture(Color clearColor, int width, int height, string id, Shader sPaintToUV, Mesh meshToDraw)
    {
        this.id = id;

        // A RenderingTexture is usually used to implement image based rendering effect
        // One typical usage of render textures is setting them as the "target texture" property of a Camera
        // This will make a camera render into a texture instead of rendering to the screen

        // RenderTextures Initialization
        runTimeTexture = new RenderTexture(width, height, 0)
        {
            anisoLevel = 0,                         // Anisotropic filtering level of the texture
            useMipMap = false,                      // To render texture as a MipMap
            filterMode = FilterMode.Bilinear        // Filtering Mode
        };

        paintedTexture = new RenderTexture(width, height, 0)
        {
            anisoLevel = 0,
            useMipMap = false,
            filterMode = FilterMode.Bilinear
        };

        //fixedIslands = new RenderTexture(paintedTexture.descriptor);

        Graphics.SetRenderTarget(runTimeTexture);   // To render into the RenderTexture
        GL.Clear(false, true, clearColor);          // To clear the active RederTexture we are drawing into.
        Graphics.SetRenderTarget(paintedTexture);
        GL.Clear(false, true, clearColor);

        // Materials initialization
        mPaintInUV = new Material(sPaintToUV);
        if (!mPaintInUV.SetPass(0)) Debug.LogError("Invalid Shader Pass: ");
        mPaintInUV.SetTexture("_BaseMap", paintedTexture);

        //mFixedEdges = new Material(sFixIslandEdges);
        //mFixedEdges.SetTexture("_IslandMap", markedIslands);
        //mFixedEdges.SetTexture("_MainTex", paintedTexture);

        // CommandBuffer Initialization

        commandBuffer = new CommandBuffer();
        commandBuffer.name = "TexturePainting" + id;

        commandBuffer.SetRenderTarget(runTimeTexture); // Command buffer holds list of rendering commands.
        commandBuffer.DrawMesh(meshToDraw, Matrix4x4.identity, mPaintInUV);

        // Adds a command to use a shader to copy the pixel data from a texture into a render texture.
        //commandBuffer.Blit(runTimeTexture, fixedIslands, mFixedEdges);
        //commandBuffer.Blit(fixedIslands, runTimeTexture);
        commandBuffer.Blit(runTimeTexture, paintedTexture);

    }

    public void SetActiveTexture(Camera mainCamera)
    {
        mainCamera.AddCommandBuffer(CameraEvent.AfterDepthTexture, commandBuffer);
    }

    public void SetInactiveTexture(Camera mainCamera)
    {
        mainCamera.RemoveCommandBuffer(CameraEvent.AfterDepthTexture, commandBuffer);
    }

    public void UpdateShaderParameters(Matrix4x4 localToWorld)
    {
        mPaintInUV.SetMatrix("mesh_Object2World", localToWorld); // Must be updated every time the mesh moves, and also at start
    }
}