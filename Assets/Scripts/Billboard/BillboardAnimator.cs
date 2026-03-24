using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class BillboardAnimator : MonoBehaviour
{
    [Header("Spritesheet")] public int cols = 9;
    public int rows = 1;
    public float fps = 8f;

    [Header("Animation range")] public int startFrame = 0;
    public int endFrame = -1;

    MeshRenderer _rend;
    MaterialPropertyBlock _block;
    static readonly int _stID = Shader.PropertyToID("_BaseMap_ST");

    float _timer;
    int _frame;

    void Awake()
    {
        _rend = GetComponent<MeshRenderer>();
        _block = new MaterialPropertyBlock();
        if (endFrame < 0) endFrame = cols * rows - 1;
    }

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer < 1f / fps) return;
        _timer = 0f;

        _frame++;
        if (_frame > endFrame) _frame = startFrame;

        ApplyFrame(_frame);
    }

    void ApplyFrame(int f)
    {
        float scaleX = 1f / cols;
        float scaleY = 1f / rows;
        float offsetX = (f % cols) * scaleX;
        float offsetY = (rows - 1 - f / cols) * scaleY; 

        _rend.GetPropertyBlock(_block);
        _block.SetVector(_stID, new Vector4(scaleX, scaleY, offsetX, offsetY));
        _rend.SetPropertyBlock(_block);
    }
}