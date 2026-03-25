using UnityEngine;

// Фантомный объект — активируется когда нет контролируемых слаймов.
// Позиционируется камерой через рейкаст на землю.
// MinionSkill использует его как цель орбиты вместо контролируемого слайма.
public class PhantomController : MonoBehaviour
{
    public static PhantomController Instance { get; private set; }

    void Awake() => Instance = this;
}
