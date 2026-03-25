using UnityEngine;

// Отдельный компонент хитбокса слайма — вешается на дочерний объект.
// Позволяет задать отдельный слой для получения урона, не меняя слой самого слайма.
public class SlimeHitbox : MonoBehaviour, IHittable
{
    [SerializeField] private SlimeMono _slime;

    public void TakeDamage(float amount) => _slime?.Data.TakeDamage(amount);
}