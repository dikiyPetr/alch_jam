using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Spawner
{
  [RequireComponent(typeof(SphereCollider))]
  public class Spawner : MonoBehaviour
  {
    [SerializeField] private float _spawnRadius;
    [SerializeField] private float _followRadius;
    [SerializeField] private float _homeRadius;

    [SerializeField] AiController[] unitPrefabs;

    private List<AiController> pool = new List<AiController>();

    void OnTriggerEnter(Collider other)
    {
      if (other.TryGetComponent<SlimeMono>(out var slime))
      {
        foreach (var unit in pool)
        {
          unit.SetState<FollowState>();
        }
      }
    }

    private void Start()
    {
      Spawn();
    }
    
    public void Spawn()
    {
      foreach (var unitPrefab in unitPrefabs)
      {
        var rand = Random.insideUnitSphere * _spawnRadius;
        var unit = Instantiate(unitPrefab, transform.position + new Vector3(rand.x, transform.position.y, rand.z), Quaternion.identity);
        pool.Add(unit);
      }
    }

    void OnDrawGizmos()
    {
#if UNITY_EDITOR
      Handles.color = Color.cyan;
      Handles.DrawWireDisc(transform.position, Vector3.up, _spawnRadius);
      Handles.color = Color.red;
      Handles.DrawWireDisc(transform.position, Vector3.up, _followRadius);
      Handles.color = Color.green;
      Handles.DrawWireDisc(transform.position, Vector3.up, _homeRadius);
#endif
    }
  }
}