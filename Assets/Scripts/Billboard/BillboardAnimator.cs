using System;
using UnityEngine;

// Воспроизводит SpriteAnimClip, сдвигая UV спрайтлиста через MaterialPropertyBlock.
// Play(name/clip) — переключить клип.
// OnEventFrame(clip, localFrame) — событие на кадрах из clip.eventFrames.
// OnClipEnd(clip) — конец не-looping клипа (остаётся на последнем кадре).
[RequireComponent(typeof(MeshRenderer))]
public class BillboardAnimator : MonoBehaviour
{
    [Header("Spritesheet")]
    [SerializeField] int _cols = 9;
    [SerializeField] int _rows = 1;

    [Header("Clips")]
    [SerializeField] SpriteAnimClip[] _clips;
    [SerializeField] SpriteAnimClip _defaultClip;

    MeshRenderer _rend;
    MaterialPropertyBlock _block;
    static readonly int StID    = Shader.PropertyToID("_BaseMap_ST");
    static readonly int ColorId = Shader.PropertyToID("_BaseColor");

    Color _flashFrom;
    float _flashTimer;
    float _flashDuration;

    SpriteAnimClip _current;
    int _localFrame;
    float _timer;

    public SpriteAnimClip CurrentClip => _current;

    // clip — текущий клип, localFrame — локальный индекс (0 = первый кадр клипа)
    public event Action<SpriteAnimClip, int> OnEventFrame;
    public event Action<SpriteAnimClip> OnClipEnd;

    void Awake()
    {
        _rend = GetComponent<MeshRenderer>();
        _block = new MaterialPropertyBlock();
    }

    void Start()
    {
        if (_defaultClip != null) Play(_defaultClip);
    }

    public void Play(string clipName, bool forceRestart = false)
    {
        foreach (var clip in _clips)
        {
            if (clip != null && clip.name == clipName)
            {
                Play(clip, forceRestart);
                return;
            }   
        }
        Debug.LogWarning($"[BillboardAnimator] Clip '{clipName}' not found on {name}");
    }

    public void Play(SpriteAnimClip clip, bool forceRestart = false)
    {
        if (clip == null || (_current == clip && !forceRestart)) return;
        _current = clip;
        _localFrame = 0;
        _timer = 0f;
        ApplyFrame(0);
    }

    // Мигнуть цветом и плавно вернуться к белому.
    public void Flash(Color color, float duration)
    {
        _flashFrom     = color;
        _flashTimer    = duration;
        _flashDuration = duration;
        SetColor(color);
    }

    void Update()
    {
        if (_flashTimer > 0f)
        {
            _flashTimer -= Time.deltaTime;
            float t = 1f - Mathf.Clamp01(_flashTimer / _flashDuration);
            SetColor(Color.Lerp(_flashFrom, Color.white, t));
        }

        if (_current == null) return;

        _timer += Time.deltaTime;
        float frameTime = 1f / _current.fps;
        if (_timer < frameTime) return;
        _timer -= frameTime;

        int clipLength = _current.endFrame - _current.startFrame + 1;
        int next = _localFrame + 1;

        if (next >= clipLength)
        {
            if (_current.loop)
                _localFrame = 0;
            else
            {
                OnClipEnd?.Invoke(_current);
                return; // остаётся на последнем кадре
            }
        }
        else
        {
            _localFrame = next;
        }

        ApplyFrame(_localFrame);
        FireEventIfNeeded(_localFrame);
    }

    void FireEventIfNeeded(int localFrame)
    {
        if (_current.eventFrames == null) return;
        foreach (var ef in _current.eventFrames)
            if (ef == localFrame) { OnEventFrame?.Invoke(_current, localFrame); return; }
    }

    void SetColor(Color color)
    {
        _rend.GetPropertyBlock(_block);
        _block.SetColor(ColorId, color);
        _rend.SetPropertyBlock(_block);
    }

    void ApplyFrame(int localFrame)
    {
        int g = _current.startFrame + localFrame;
        float sx = 1f / _cols;
        float sy = 1f / _rows;
        float ox = (g % _cols) * sx;
        float oy = (_rows - 1 - g / _cols) * sy;

        _rend.GetPropertyBlock(_block);
        _block.SetVector(StID, new Vector4(sx, sy, ox, oy));
        _rend.SetPropertyBlock(_block);
    }
}
