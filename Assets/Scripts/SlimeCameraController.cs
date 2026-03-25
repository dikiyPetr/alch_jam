using UnityEngine;

// Вешается на главную камеру.
// Режим 1 (есть контролируемый слайм): камера смещается так, чтобы все слаймы
//   помещались в кадр; зум пропорционален разбросу.
// Режим 2 (нет контролируемого слайма): камера движется по вводу игрока,
//   фантом-объект ставится в центр кадра (рейкаст на плоскость Y=0).
public class SlimeCameraController : MonoBehaviour
{
    [SerializeField] PhantomController _phantom;

    [Header("Режим с контролируемым слаймом")]
    [SerializeField] Vector3 _baseOffset = new(0f, 12f, -8f);
    [SerializeField] float _referenceSpread = 3f;   // при таком разбросе — базовый масштаб
    [SerializeField] float _maxZoomScale = 4f;
    [SerializeField] float _smoothSpeed = 5f;

    [Header("Режим без контролируемого слайма")]
    [SerializeField] float _freeCameraSpeed = 8f;

    Camera _cam;
    Vector3 _smoothVelocity;

    void Awake()
    {
        _cam = GetComponent<Camera>();
    }

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

        // Максимальное расстояние от центроида (на плоскости XZ)
        float maxSpread = 0f;
        foreach (var s in allMono)
        {
            if (s == null) continue;
            var diff = s.transform.position - centroid;
            diff.y = 0f;
            maxSpread = Mathf.Max(maxSpread, diff.magnitude);
        }

        // Масштабируем offset пропорционально разбросу
        float scale = Mathf.Clamp(maxSpread / Mathf.Max(_referenceSpread, 0.01f), 1f, _maxZoomScale);
        var targetPos = centroid + _baseOffset * scale;

        transform.position = Vector3.SmoothDamp(
            transform.position, targetPos, ref _smoothVelocity, 1f / _smoothSpeed);
    }

    void UpdatePhantomMode()
    {
        _phantom.gameObject.SetActive(true);

        // Двигаем камеру вводом игрока (на плоскости XZ, сохраняем высоту)
        var input = SlimePoolMono.Instance.MoveInput;
        var move = new Vector3(input.x, 0f, input.y) * (_freeCameraSpeed * Time.deltaTime);
        transform.position += move;

        // Помещаем фантом в центр кадра (рейкаст на плоскость Y=0)
        var ray = _cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        var groundPlane = new Plane(Vector3.up, Vector3.zero);
        if (groundPlane.Raycast(ray, out float dist))
            _phantom.transform.position = ray.GetPoint(dist);
    }
}
