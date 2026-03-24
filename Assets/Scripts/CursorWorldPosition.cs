using UnityEngine;
using UnityEngine.InputSystem;

public class CursorWorldPosition : MonoBehaviour
{
    public static CursorWorldPosition Instance { get; private set; }

    public Vector3 Position { get; private set; }

    Camera _cam;

    void Awake()
    {
        Instance = this;
        _cam = Camera.main;
    }

    void Update()
    {
        var mouse = Mouse.current;
        if (mouse == null) return;

        var ray = _cam.ScreenPointToRay(mouse.position.ReadValue());
        var groundPlane = new Plane(Vector3.up, Vector3.zero);
        if (groundPlane.Raycast(ray, out float dist))
            Position = ray.GetPoint(dist);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(Position, 0.15f);
    }
}
