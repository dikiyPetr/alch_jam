using UnityEngine;

[CreateAssetMenu(fileName = "SlimeConfig", menuName = "Slime/Config")]
public class SlimeConfig : ScriptableObject
{
    static SlimeConfig _instance;
    public static SlimeConfig Instance => _instance;

    public static void Init(SlimeConfig config) => _instance = config;

    [Header("Юниты")]
    public float hpPerUnit = 50f;
    public float initialMaxHp = 500f;

    [Header("Движение")]
    public float moveSpeed = 5f;
    public float mergeSpeedMultiplier = 1.5f;

    [Header("Визуал")]
    public float baseUnitScale = 0.5f;

    [Header("Деление / Слияние")]
    public float splitSpawnRadius = 1.5f;
    public float mergeRadius = 0.5f;
    // Начальная скорость выброса при делении
    public float splitImpulseSpeed = 3f;
    // Задержка между выбросами юнитов при максимальном делении
    public float maxSplitInterval = 0.12f;
    // Сколько секунд слайм свободно летит после выброса (до перехода к обычной логике)
    public float splitKickDuration = 0.4f;

    [Header("Скилл")]
    public float skillDuration = 2f;

    [Header("Проджектайл")]
    public float projectileSpeedMultiplier = 3f;
    public float projectileReturnDelay = 2f;
    // Насколько быстро снаряд разворачивается к цели в фазе возврата (коэф. lerp в секунду)
    public float projectileSteeringSpeed = 4f;

    [Header("Миньон AI")]
    public float minionOrbitRadius = 3f;
    public float minionOrbitSpeedMultiplier = 0.8f;
    public float minionAttackScanRadius = 5f;
    public float minionAttackLeashRadius = 7f;
    public float minionAttackDamage = 10f;
    public float minionAttackCooldown = 0.5f;
    public float minionAttackRange = 1.5f;

    [Header("Рейкаст")]
    public LayerMask groundLayer;

    [Header("Слои урона")]
    // Слои, на которых находятся коллайдеры/триггеры источников урона
    public LayerMask playerDamageSourceLayer;
    public LayerMask enemyDamageSourceLayer;
    // Маски целей: что бьёт игрок, что бьют враги
    public LayerMask playerTargetLayers;
    public LayerMask enemyTargetLayers;
}
