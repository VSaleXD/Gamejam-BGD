using UnityEngine;

public class PowerSocket : MonoBehaviour
{
    public enum SocketColor
    {
        Red,
        Blue,
        Green,
        Yellow
    }

    [Header("Visual")]
    [SerializeField] private SpriteRenderer spriteRender;

    [SerializeField] private Sprite spriteRed;
    [SerializeField] private Sprite spriteBlue;
    [SerializeField] private Sprite spriteGreen;
    [SerializeField] private Sprite spriteYellow;

    public SocketColor currentColor;

    public bool isPowered = false;

    public void SetColor(SocketColor color)
    {
        currentColor = color;

        if (spriteRender == null)
            spriteRender = GetComponent<SpriteRenderer>();

        switch (color)
        {
            case SocketColor.Red:
                spriteRender.sprite = spriteRed;
                break;
            case SocketColor.Blue:
                spriteRender.sprite = spriteBlue;
                break;
            case SocketColor.Green:
                spriteRender.sprite = spriteGreen;
                break;
            case SocketColor.Yellow:
                spriteRender.sprite = spriteYellow;
                break;
        }
    }

    public void PowerOn()
    {
        if (isPowered) return;
        isPowered = true;
    }

    public void ResetPower()
    {
        isPowered = false;
    }
}