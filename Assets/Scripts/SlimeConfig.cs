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

    [Header("Скилл")]
    public float skillDuration = 2f;

    [Header("Проджектайл")]
    public float projectileSpeedMultiplier = 3f;
    public float projectileReturnDelay = 2f;
    // Насколько быстро снаряд разворачивается к цели в фазе возврата (коэф. lerp в секунду)
    public float projectileSteeringSpeed = 4f;

    [Header("Рейкаст")]
    public LayerMask groundLayer;
}
