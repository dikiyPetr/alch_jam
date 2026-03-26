using System;
using UnityEngine;

// Стрела — летит к цели, поворачивая спрайт в её направлении.
// Урон наносится при достижении цели. Подписчики получают уведомление через OnHit.
public class ArrowProjectile : MonoBehaviour
{
    float _speed;
    [SerializeField] float _arrivalRadius = 0.4f;
    // Дочерний объект со спрайтом — поворачивается в сторону цели
    [SerializeField] Transform _sprite;
    [SerializeField] Vector3 _spriteRotationOffset;

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
        if (_sprite == null || dir == Vector3.zero) return;
        _sprite.rotation = Quaternion.LookRotation(dir, Vector3.up) * Quaternion.Euler(_spriteRotationOffset);
    }

    void Hit()
    {
        if (Target.TryGetComponent<IHittable>(out var hittable))
            hittable.TakeDamage(_damage);
        OnHit?.Invoke(_damage);
        Destroy(gameObject);
    }
}
