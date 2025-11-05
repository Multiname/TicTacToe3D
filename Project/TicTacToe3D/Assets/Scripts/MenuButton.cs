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

        button.onClick.AddListener(() => handleClick());
    }

    public void SetShowMenuSprite() {
        image.sprite = showMenuSprite;
    }

    public void SetGoBackSprite() {
        image.sprite = goBackSprite;
    }
}
