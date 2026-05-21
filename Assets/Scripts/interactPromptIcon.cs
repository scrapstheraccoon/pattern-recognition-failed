using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class interactPromptIcon : MonoBehaviour
{
    [SerializeField] private Sprite keyboardSprite;
    [SerializeField] private Sprite gamepadSprite;

    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    void Update()
    {
        if (Gamepad.current != null)
            image.sprite = gamepadSprite;
        else
            image.sprite = keyboardSprite;
    }
}
