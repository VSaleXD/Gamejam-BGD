using UnityEngine;
using UnityEngine.Events;

public class PowerSocket : MonoBehaviour, IInteractable
{
    public enum SocketColor
    {
        Default,
        Red,
        Blue,
        Green,
        Yellow
    }

    [SerializeField] private UnityEvent onSocketPowered;

    [Header("Color Mode")]
    [SerializeField] private bool useRandomColor = true;

    [SerializeField] private SocketColor fixedColor = SocketColor.Red;

    [Header("Visual")]
    [SerializeField] private SpriteRenderer spriteRender;
    [SerializeField] private Sprite spriteDefault;
    [SerializeField] private Sprite spriteRed;
    [SerializeField] private Sprite spriteBlue;
    [SerializeField] private Sprite spriteGreen;
    [SerializeField] private Sprite spriteYellow;


    public SocketColor currentColor;

    public bool isPowered = false;

    public void Interact(GameObject interactor)
    {
        PlayerCable pc = interactor.GetComponentInChildren<PlayerCable>();

        if (pc != null)
        {
            pc.TryAttach(this);
        }
    }

    public void SetColor(SocketColor color)
    {
        currentColor = color;

        if (spriteRender == null)
            spriteRender = GetComponent<SpriteRenderer>();

        switch (color)
        {
            case SocketColor.Default:
                spriteRender.sprite = spriteDefault;
                break;

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

        Debug.Log(name + " SOCKET AKTIF");

        // ⭐⭐⭐ DI SINI SOCKET TRIGGER SESUATU
        onSocketPowered?.Invoke();
    }

    public void ResetPower()
    {
        isPowered = false;
    }

    public bool CanAccept(CableInstance cable)
    {
        Color c = cable.GetCableColor();

        switch (currentColor)
        {
            case SocketColor.Default:
                return c == Color.gray;

            case SocketColor.Red:
                return c == Color.red;

            case SocketColor.Blue:
                return c == Color.blue;

            case SocketColor.Green:
                return c == Color.green;

            case SocketColor.Yellow:
                return c == Color.yellow;
        }

        return false;
    }

    private void Awake()
    {
        // ⭐ kalau tidak random → set warna langsung
        if (!useRandomColor)
        {
            SetColor(fixedColor);
        }
    }

    public bool UseRandomColor()
    {
        return useRandomColor;
    }
}