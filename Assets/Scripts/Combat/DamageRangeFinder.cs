using System.Collections.Generic;
using UnityEngine;

// Отдельный компонент для отслеживания IHittable-целей в триггер-зоне.
// Живёт на отдельном GameObject с Collider-ом — позволяет задать свой слой и позицию.
// DamageSourceMono и наследники читают InRange из этого компонента.
[RequireComponent(typeof(Collider))]
public class DamageRangeFinder : MonoBehaviour
{
    Collider _collider;

    public Collider Collider => _collider;
    [SerializeField] public LayerMask targetLayers;
    public HashSet<IHittable> InRange { get; } = new();

    void Awake()
    {
        _collider = GetComponent<Collider>();
        _collider.isTrigger = true;
    }

    public void RemoveDeadEntries()
        => InRange.RemoveWhere(t => t is Object obj && !obj);

    void OnTriggerEnter(Collider other)
    {
        if (IsInLayerMask(other.gameObject.layer) && other.TryGetComponent<IHittable>(out var target))
            InRange.Add(target);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<IHittable>(out var target))
            InRange.Remove(target);
    }

    bool IsInLayerMask(int layer) => (targetLayers.value & (1 << layer)) != 0;
}