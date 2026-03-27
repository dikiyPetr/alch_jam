using System;
using UnityEngine;

// Стрела — летит к цели, поворачивая спрайт в её направлении.
// Урон наносится при достижении цели. Подписчики получают уведомление через OnHit.
public class ArrowProjectile : MonoBehaviour
{
    float _speed;
    [SerializeField] float _arrivalRadius = 0.4f;
    [SerializeField] BillboardSprite _billboard;

    public Transform Target { get; private set; }

    public event Action<float> OnHit;

    float _damage;

    public void Launch(Transform target, float damage, float speed)
    {
        Target = target;
        _damage = damage;
        _speed = speed;
    }

    void Update()
    {
        if (Target == null) { Destroy(gameObject); return; }

        var dir = Target.position - transform.position;
        dir.y = 0f;

        OrientSprite(dir);

        if (dir.sqrMagnitude <= _arrivalRadius * _arrivalRadius)
        {
            Hit();
            return;
        }

        transform.position += dir.normalized * (_speed * Time.deltaTime);
    }

    void OrientSprite(Vector3 dir)
    {
        _billboard?.SetDirection(dir);
    }

    void Hit()
    {
        if (Target.TryGetComponent<IHittable>(out var hittable))
            hittable.TakeDamage(_damage);
        OnHit?.Invoke(_damage);
        Destroy(gameObject);
    }
}
