using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Image))]
public class MenuButton : MonoBehaviour {
    [SerializeField] Sprite showMenuSprite;
    [SerializeField] Sprite goBackSprite;

    private Button button;
    private Image image;
    public Action handleClick = () => { };

    private void Awake() {
        button = GetComponent<Button>();
        image = GetComponent<Image>();

        button.onClick.AddListener(() => {
            handleClick();
            SfxPlayer.PlaySound(SfxPlayer.Sound.UI_BUTTON_CLICK);
        });
    }

    public void SetShowMenuSprite() {
        image.sprite = showMenuSprite;
    }

    public void SetGoBackSprite() {
        image.sprite = goBackSprite;
    }
}
