using UnityEngine;

// Отображает int-поле как выпадающий список слоёв Unity в инспекторе.
// Используй вместе с [SerializeField] int:
//   [Layer, SerializeField] int _layer;
public class LayerAttribute : PropertyAttribute { }
