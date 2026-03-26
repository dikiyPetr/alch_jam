using UnityEngine;

// Ортографическая камера под углом.
// Режим 1 (есть контролируемые слаймы): центрирует кадр на центроиде всех слаймов,
//   меняет orthographicSize по разбросу. Позиция вычисляется через луч камеры → Y=0,
//   так что угол наклона учитывается автоматически.
// Режим 2 (нет контролируемых): камера движется вводом, фантом — в центр кадра.
public class SlimeCameraController : MonoBehaviour
{
    [SerializeField] PhantomController _phantom;

    [Header("Позиция")]
    [SerializeField] float _cameraHeight = 20f;    // высота камеры над плоскостью Y=0
    [SerializeField] float _smoothSpeed = 5f;

    [Header("Зум (orthographicSize)")]
    [SerializeField] float _minOrthoSize = 4f;
    [SerializeField] float _baseOrthoSize = 6f;
    [SerializeField] float _maxOrthoSize = 18f;
    [SerializeField] float _referenceSpread = 5f;  // разброс при котором достигается _maxOrthoSize
    [SerializeField] float _zoomSmoothSpeed = 4f;

    [Header("Режим без контролируемых")]
    [SerializeField] float _freeCameraSpeed = 8f;
    [SerializeField] float _minFreeCameraSpeed = 1f;
    [SerializeField] float _speedFalloffDistance = 15f; // дистанция до ближайшего слайма при которой скорость минимальна

    Camera _cam;
    Vector3 _smoothVelocity;
    float _orthoVelocity;

    void Awake() => _cam = GetComponent<Camera>();

    void LateUpdate()
    {
        if (SlimePoolMono.Instance == null) return;

        var controlled = SlimePoolMono.Instance.GetControlledSlimes();
        if (controlled.Count > 0)
            UpdateControlledMode();
        else
            UpdatePhantomMode();
    }

    void UpdateControlledMode()
    {
        _phantom.gameObject.SetActive(false);

        var allMono = SlimePoolMono.Instance.AllMono;
        if (allMono.Count == 0) return;

        // Центроид всех слаймов
        var centroid = Vector3.zero;
        int count = 0;
        foreach (var s in allMono)
        {
            if (s == null) continue;
            centroid += s.transform.position;
            count++;
        }
        if (count == 0) return;
        centroid /= count;

        // Средний разброс от центроида по плоскости XZ.
        // Среднее устойчивее к выбросам (отлетевший слайм при делении не доминирует).
        float totalSpread = 0f;
        foreach (var s in allMono)
        {
            if (s == null) continue;
            var diff = s.transform.position - centroid;
            diff.y = 0f;
            totalSpread += diff.magnitude;
        }
        float avgSpread = totalSpread / count;

        // Вычисляем позицию камеры через луч: нужно чтобы центр кадра смотрел на centroid.
        // Решаем: camPos + t * forward = (centroid.x, 0, centroid.z), где camPos.y = _cameraHeight
        var forward = transform.forward;
        if (Mathf.Abs(forward.y) < 0.001f) return; // камера горизонтальна — пропускаем
        float t = -_cameraHeight / forward.y;
        var targetPos = new Vector3(
            centroid.x - t * forward.x,
            _cameraHeight,
            centroid.z - t * forward.z
        );

        transform.position = Vector3.SmoothDamp(
            transform.position, targetPos, ref _smoothVelocity, 1f / _smoothSpeed);

        // orthographicSize — линейно от среднего разброса, зажато в [_minOrthoSize, _maxOrthoSize]
        float ratio = Mathf.Clamp01(avgSpread / Mathf.Max(_referenceSpread, 0.01f));
        float targetOrtho = Mathf.Clamp(
            Mathf.Lerp(_baseOrthoSize, _maxOrthoSize, ratio),
            _minOrthoSize, _maxOrthoSize);
        _cam.orthographicSize = Mathf.SmoothDamp(
            _cam.orthographicSize, targetOrtho, ref _orthoVelocity, 1f / _zoomSmoothSpeed);
    }

    void UpdatePhantomMode()
    {
        _phantom.gameObject.SetActive(true);

        var input = SlimePoolMono.Instance.MoveInput;
        float speed = CalcFreeCameraSpeed();
        transform.position += new Vector3(input.x, 0f, input.y) * (speed * Time.deltaTime);

        // Фантом в центр кадра через пересечение луча с плоскостью Y=0
        var ray = _cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        var groundPlane = new Plane(Vector3.up, Vector3.zero);
        if (groundPlane.Raycast(ray, out float dist))
            _phantom.transform.position = ray.GetPoint(dist);
    }

    float CalcFreeCameraSpeed()
    {
        var allMono = SlimePoolMono.Instance.AllMono;
        if (allMono.Count == 0) return _minFreeCameraSpeed;

        var phantomPos = _phantom.transform.position;
        float minSqDist = float.MaxValue;
        foreach (var s in allMono)
        {
            if (s == null) continue;
            var diff = s.transform.position - phantomPos;
            diff.y = 0f;
            minSqDist = Mathf.Min(minSqDist, diff.sqrMagnitude);
        }

        float t = Mathf.Clamp01(Mathf.Sqrt(minSqDist) / Mathf.Max(_speedFalloffDistance, 0.01f));
        return Mathf.Lerp(_freeCameraSpeed, _minFreeCameraSpeed, t);
    }
}
