using UnityEngine;

public class BillboardSprite : MonoBehaviour
{
    [SerializeField] bool lockX = false;
    [SerializeField] bool lockZ = false;
    // Коррекция угла: 0 если спрайт по умолчанию смотрит вправо (как стрела).
    [SerializeField] float _directionAngleOffset = 0f;

    Camera _cam;
    Vector3 _worldDir;
    bool _hasDirection;

    void Start() => _cam = Camera.main;

    // Задать мировое направление для поворота спрайта в плоскости билборда.
    // Вызывать каждый кадр из объекта (ArrowProjectile и т.п.).
    public void SetDirection(Vector3 worldDir)
    {
        _hasDirection = worldDir.sqrMagnitude > 0.0001f;
        _worldDir = worldDir;
    }

    void LateUpdate()
    {
        Vector3 camDir = _cam.transform.forward;
        if (lockX) camDir.x = 0;
        if (lockZ) camDir.z = 0;

        if (camDir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(camDir, Vector3.up);

        if (!_hasDirection) return;

        // Проецируем мировое направление на плоскость экрана (right/up камеры).
        var n = _worldDir.normalized;
        float projRight = Vector3.Dot(n, _cam.transform.right);
        float projUp    = Vector3.Dot(n, _cam.transform.up);
        float angle     = Mathf.Atan2(projUp, projRight) * Mathf.Rad2Deg + _directionAngleOffset;

        // Поворот вокруг локального Z билборда (ось смотрит в камеру).
        transform.rotation *= Quaternion.Euler(0f, 0f, angle);
    }
}
