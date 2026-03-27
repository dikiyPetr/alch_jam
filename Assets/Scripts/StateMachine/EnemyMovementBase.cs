using UnityEngine;

// Абстрактный компонент поведения перемещения врага.
// Определяет логику движения и условие готовности к атаке.
// Вызывается из EnemyMoveState и EnemyAttackState.
public abstract class EnemyMovementBase : MonoBehaviour
{
    // Вернуть true, когда враг занял позицию для атаки (вход в AttackState).
    public abstract bool IsInPosition(Transform self, Transform target);

    // Вернуть true, пока позиция считается допустимой для продолжения атаки.
    // По умолчанию — те же границы что и IsInPosition.
    // Переопределяй чтобы добавить гистерезис и избежать мерцания состояний.
    public virtual bool IsStillInPosition(Transform self, Transform target)
        => IsInPosition(self, target);

    // Двигать unit к позиции атаки относительно цели.
    public abstract void UpdateMovement(UnitMono unit, Transform target);

    // Сбросить инерцию при выходе из состояния движения.
    public virtual void ResetMovement(UnitMono unit) => unit.StopMovement();
}
