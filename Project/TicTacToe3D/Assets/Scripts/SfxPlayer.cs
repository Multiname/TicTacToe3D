using UnityEngine;

public class SfxPlayer : MonoBehaviour {
    [SerializeField] AudioSource gameStartAudioSource;
    [SerializeField, Range(0, 1)] float gameStartBaseVolume = 1.0f;
    [SerializeField] AudioSource gameEndAudioSource;
    [SerializeField, Range(0, 1)] float gameEndBaseVolume = 1.0f;
    [SerializeField] AudioSource gameDrawAudioSource;
    [SerializeField, Range(0, 1)] float gameDrawBaseVolume = 1.0f;

    [SerializeField] AudioSource moveSelectionFigureAudioSource;
    [SerializeField, Range(0, 1)] float moveSelectionFigureBaseVolume = 1.0f;
    [SerializeField] AudioSource placeFigureAudioSource;
    [SerializeField, Range(0, 1)] float placeFigureBaseVolume = 1.0f;
    [SerializeField] AudioSource figureFallAudioSource;
    [SerializeField, Range(0, 1)] float figureFallBaseVolume = 1.0f;
    [SerializeField] AudioSource destroyFigureAudioSource;
    [SerializeField, Range(0, 1)] float destroyFigureBaseVolume = 1.0f;

    [SerializeField] AudioSource switchVisibilityOfPointsEffectAudioSource;
    [SerializeField, Range(0, 1)] float switchVisibilityOfPointsEffectBaseVolume = 1.0f;
    [SerializeField] AudioSource getBonusAudioSource;
    [SerializeField, Range(0, 1)] float getBonusBaseVolume = 1.0f;
    [SerializeField] AudioSource bonusTurnAudioSource;
    [SerializeField, Range(0, 1)] float bonusTurnBaseVolume = 1.0f;

    [SerializeField] AudioSource uiButtonHoverAudioSource;
    [SerializeField, Range(0, 1)] float uiButtonHoverBaseVolume = 1.0f;
    [SerializeField] AudioSource uiButtonClickAudioSource;
    [SerializeField, Range(0, 1)] float uiButtonClickBaseVolume = 1.0f;

    private static AudioSource[] audioSources;
    private static float[] baseVolumes;
    private const int NUMBER_OF_SOUNDS = 12;
    public enum Sound {
        GAME_START,
        GAME_END,
        GAME_DRAW,
        MOVE_SELECTION_FIGURE,
        PLACE_FIGURE,
        FIGURE_FALL,
        DESTROY_FIGURE,
        SWITCH_VISIBILITY_OF_POINTS_EFFECT,
        GET_BONUS,
        BONUS_TURN,
        UI_BUTTON_HOVER,
        UI_BUTTON_CLICK
    }

    private void Awake() {
        audioSources = new AudioSource[NUMBER_OF_SOUNDS];
        baseVolumes = new float[NUMBER_OF_SOUNDS];

        audioSources[(int)Sound.GAME_START] = gameStartAudioSource;
        baseVolumes[(int)Sound.GAME_START] = gameStartBaseVolume;

        audioSources[(int)Sound.GAME_END] = gameEndAudioSource;
        baseVolumes[(int)Sound.GAME_END] = gameEndBaseVolume;

        audioSources[(int)Sound.GAME_DRAW] = gameDrawAudioSource;
        baseVolumes[(int)Sound.GAME_DRAW] = gameDrawBaseVolume;


        audioSources[(int)Sound.MOVE_SELECTION_FIGURE] = moveSelectionFigureAudioSource;
        baseVolumes[(int)Sound.MOVE_SELECTION_FIGURE] = moveSelectionFigureBaseVolume;

        audioSources[(int)Sound.PLACE_FIGURE] = placeFigureAudioSource;
        baseVolumes[(int)Sound.PLACE_FIGURE] = placeFigureBaseVolume;
        
        audioSources[(int)Sound.FIGURE_FALL] = figureFallAudioSource;
        baseVolumes[(int)Sound.FIGURE_FALL] = figureFallBaseVolume;

        audioSources[(int)Sound.DESTROY_FIGURE] = destroyFigureAudioSource;
        baseVolumes[(int)Sound.DESTROY_FIGURE] = destroyFigureBaseVolume;


        audioSources[(int)Sound.SWITCH_VISIBILITY_OF_POINTS_EFFECT] = switchVisibilityOfPointsEffectAudioSource;
        baseVolumes[(int)Sound.SWITCH_VISIBILITY_OF_POINTS_EFFECT] = switchVisibilityOfPointsEffectBaseVolume;

        audioSources[(int)Sound.GET_BONUS] = getBonusAudioSource;
        baseVolumes[(int)Sound.GET_BONUS] = getBonusBaseVolume;

        audioSources[(int)Sound.BONUS_TURN] = bonusTurnAudioSource;
        baseVolumes[(int)Sound.BONUS_TURN] = bonusTurnBaseVolume;


        audioSources[(int)Sound.UI_BUTTON_HOVER] = uiButtonHoverAudioSource;
        baseVolumes[(int)Sound.UI_BUTTON_HOVER] = uiButtonHoverBaseVolume;

        audioSources[(int)Sound.UI_BUTTON_CLICK] = uiButtonClickAudioSource;
        baseVolumes[(int)Sound.UI_BUTTON_CLICK] = uiButtonClickBaseVolume;
    }

    public static void PlaySound(Sound sound) {
        var audioSource = audioSources[(int)sound];
        audioSource.volume = baseVolumes[(int)sound] * GameSettings.volume;
        audioSource.Play();
    }
}
