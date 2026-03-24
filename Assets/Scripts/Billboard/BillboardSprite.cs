using UnityEngine;

public class BillboardSprite : MonoBehaviour
{
    [SerializeField] bool lockX = false; 
    [SerializeField] bool lockZ = false;
    
    Camera _cam;

    void Start() => _cam = Camera.main;

    void LateUpdate()
    {
        Vector3 dir = _cam.transform.forward;
        
        if (lockX) dir.x = 0;
        if (lockZ) dir.z = 0;
        
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
    }
}
