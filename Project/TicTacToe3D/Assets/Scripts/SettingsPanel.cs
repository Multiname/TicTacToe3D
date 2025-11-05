using System;
using UnityEngine;
using UnityEngine.UIElements;

public class SettingsPanel : UiPanel {
    [SerializeField] GameSettings gameSettings;

    protected override void Awake() {
        base.Awake();

        ConfigureSlider("VolumeSlider", gameSettings.volume, value => gameSettings.volume = value);
        ConfigureSlider("SensitivitySlider", gameSettings.sensitivity, value => gameSettings.sensitivity = value);
        ConfigureSlider("EffectsSpeedSlider", gameSettings.effectsSpeed, value => gameSettings.effectsSpeed = value);
    }

    private void ConfigureSlider(string sliderName, float initialValue, Action<float> onValueChangedAction) {
        var slider = uiDocument.rootVisualElement.Q<Slider>(sliderName);
        slider.value = initialValue;
        slider.RegisterValueChangedCallback(evt => onValueChangedAction(evt.newValue));
    }
}
