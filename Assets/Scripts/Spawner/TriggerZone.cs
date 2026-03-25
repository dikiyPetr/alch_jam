using System;
using UnityEngine;

public class TriggerZone : MonoBehaviour
{
  private void Awake()
  {
    _group = GetComponentInParent<Group>();
  }

  public enum TriggerType
  {
    Attack,
    Home
  }

  public TriggerType type;
  private Group _group;

  private void OnTriggerEnter(Collider other)
  {
    _group.OnTriggerZoneEnter(type, other);
  }

  private void OnTriggerExit(Collider other)
  {
    _group.OnTriggerZoneExit(type, other);
  }
}