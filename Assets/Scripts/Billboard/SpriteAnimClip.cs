using UnityEngine;

// Данные одного анимационного клипа для BillboardAnimator.
// startFrame/endFrame — глобальные индексы кадров на спрайтлисте.
// eventFrames — локальные индексы (0 = первый кадр клипа), на которых срабатывает OnEventFrame.
[CreateAssetMenu(fileName = "NewAnimClip", menuName = "Billboard/Anim Clip")]
public class SpriteAnimClip : ScriptableObject
{
    public int startFrame;
    public int endFrame;
    public float fps = 8f;
    public bool loop = true;
    [Tooltip("Локальные индексы (0 = первый кадр клипа), на которых срабатывает OnEventFrame")]
    public int[] eventFrames;
}
