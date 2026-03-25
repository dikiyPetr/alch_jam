using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class Group : MonoBehaviour
{
  [SerializeField] private float _spawnRadius;
  [SerializeField] private float _followRadius;
  [SerializeField] private float _homeRadius;

  [SerializeField] DummyAi[] unitPrefabs;

  private List<DummyAi> pool = new List<DummyAi>();

  public void OnTriggerZoneEnter(TriggerZone.TriggerType type, Collider other)
  {
    if (type == TriggerZone.TriggerType.Attack)
    {
      if (other.TryGetComponent<SlimeMono>(out var slime))
      {
        foreach (var unit in pool)
        {
          unit.SetState<FollowState>();
        }
      }
    }
  }

  public void OnTriggerZoneExit(TriggerZone.TriggerType type, Collider other)
  {
    if (type == TriggerZone.TriggerType.Home)
    {
      if (other.TryGetComponent<AiController>(out var slime))
      {
        foreach (var unit in pool)
        {
          unit.SetState<MoveHomeState>();
        }
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
      unit.SetGroupToHomeData(this);
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