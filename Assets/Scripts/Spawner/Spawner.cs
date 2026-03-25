using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Spawner
{
  public class Spawner : MonoBehaviour
  {
    [SerializeField] private float _spawnRadius;
    [SerializeField] private float _followRadius;
    [SerializeField] private float _homeRadius;

    [SerializeField] AiController[] unitPrefabs;
    private List<AiController> pool = new List<AiController>();
    private void Start()
    {
      Spawn();
      StartCheck().Forget();
    }
    
    private async UniTaskVoid StartCheck()
    {
      var token = this.GetCancellationTokenOnDestroy();

      while (true)
      {
        CheckTarget();
        await UniTask.Delay(1000, cancellationToken: token);
      }
    }
    
    private void CheckTarget()
    {
      var distanceToSlime = (transform.position - SlimePoolMono.Instance?.GetControlledSlimes().First()?.transform.position)?.sqrMagnitude;
      if (distanceToSlime.HasValue && distanceToSlime < _followRadius * _followRadius)
      {
        foreach (var unit in pool)
        {
          unit.SetState<FollowState>();
        }

      }

      foreach (var unit in pool)
      {
        var distance = (transform.position - unit.transform.position).sqrMagnitude;

        if(distance > _homeRadius * _homeRadius) unit.SetState<MoveHomeState>();
      }
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