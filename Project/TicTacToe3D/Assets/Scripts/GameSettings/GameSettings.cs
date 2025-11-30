using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Scriptable Objects/GameSettings")]
public class GameSettings : ScriptableObject {
    public const int NUMBER_OF_RULES = 5;
    public const int NUMBER_OF_MODIFIERS = 7;

    public enum Modifier {
        BASE_POINTS,
        Combo,
        LINE_2D,
        LINE_3D,
        FALL,
        HEIGHT,
        BONUS_TURN
    }

    public enum InterceptionRule {
        NO_RESTRICTION,
        NO_POINTS,
        NO_BONUS,
        STEAL_BASE_POINTS,
        STEAL_ALL_POINTS
    }

    [field: SerializeField] public int MaxWinPointsValue { get; private set; } = 99;
    public int winPoints = 1;

    public readonly bool[] enabledModifiers = new bool[NUMBER_OF_MODIFIERS] {
        true,
        false,
        false,
        false,
        false,
        false,
        false,
    };

    [HideInInspector] public InterceptionRule enabledInterceptionRule = InterceptionRule.NO_RESTRICTION;

    public static float volume = 1.0f;
    [HideInInspector] public float sensitivity = 1.0f;

    private float _effectsSpeed = 1.0f;
    public float EffectsSpeed {
        get => _effectsSpeed;
        set {
            _effectsSpeed = value;
            BackgroundParticle.settingsEffectsSpeed = value;
        }
    }
}
