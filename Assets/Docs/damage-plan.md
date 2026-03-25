# План: урон слаймов + манекен для тестирования

---

## 1. Интерфейс IHittable

**Файл:** `Assets/Scripts/Combat/IHittable.cs`

```csharp
public interface IHittable
{
    void TakeDamage(float amount, UnitMono source);
}
```

Любой объект, принимающий урон, реализует этот интерфейс.

---

## 2. SlimeStats — класс статов

**Файл:** `Assets/Scripts/Slime/SlimeStats.cs`

Отдельный `[Serializable]` класс, хранит параметры урона.
Ссылка — `[SerializeField] SlimeStats _stats` внутри `Slime`.

| Поле | Тип | Описание |
|---|---|---|
| `contactDamage` | float | Урон при контакте (единоразово при входе в коллизию) |
| `radiusDamage` | float | Урон в тике по площади |
| `radiusRange` | float | Радиус поражения |
| `radiusTickRate` | float | Интервал тиков (сек) |

Слайм масштабирует урон от `ContainedUnitCount` — множитель на стороне наносящего урон.

---

## 3. Урон при контакте

**Где:** `SlimeMono.cs`, метод `OnCollisionEnter(Collision col)`

- `col.gameObject.TryGetComponent<IHittable>` → `TakeDamage(contactDamage * unitCount, this)`
- Не повторяется до следующего `OnCollisionExit` (флаг или HashSet активных контактов)
- Только для объектов **не из пула слаймов** (проверка `SlimePoolMono.Instance`)

---

## 4. Урон в радиусе

**Вариант А — пассивное поведение в Update SlimeMono:**
- Таймер тика, `Physics.OverlapSphere` по `radiusRange`
- По каждому попаданию: `TryGetComponent<IHittable>` → урон
- Слои через `LayerMask` (манекены на отдельном слое)

**Вариант Б — отдельный `RadiusAttackSkill`:**
- Активируется извне как обычный скилл
- Завершается по условию (таймер, смерть цели и т.д.)

> Рекомендую **Вариант А** — радиусный урон пассивен и всегда активен.

---

## 5. Манекен DummyMono

**Файл:** `Assets/Scripts/Combat/DummyMono.cs`
**Префаб:** создать в редакторе, добавить `DummyMono`, Collider, World Space Canvas

Реализует `IHittable`. Собирает статистику:

| Поле | Описание |
|---|---|
| `totalDamage` | Суммарный урон за сессию |
| `lastHit` | Урон последнего удара |
| `dps` | Скользящее среднее за последние N секунд (окно ~3–5 сек) |

### UI на манекене
- **World Space Canvas** дочерним объектом
- TextMeshPro: три строки (Total / Last / DPS)
- Обновляется в `Update()` каждый кадр
- Опционально: всплывающие цифры урона (`FloatingText`) через пул префабов

---

## 6. Порядок реализации

1. `IHittable` — интерфейс (5 мин)
2. `SlimeStats` — данные, подключить в `Slime` (15 мин)
3. `DummyMono` — статистика без UI (15 мин)
4. Контактный урон в `SlimeMono.OnCollisionEnter` (20 мин)
5. Радиусный урон в `SlimeMono.Update` (20 мин)
6. UI на манекене — текст + DPS окно (20 мин)
7. Настройка префаба манекена в редакторе (10 мин)

---

## Зависимости между компонентами

```
SlimeStats  ──────► Slime
                      │
              ┌───────┴────────┐
              ▼                ▼
     OnCollisionEnter     Update (тик)
     (контакт)            (радиус)
              │                │
              └───────┬────────┘
                      ▼
                 IHittable
                      │
                      ▼
                 DummyMono
                 (total / last / dps)
```
