using UnityEngine;

public class PowerSocket : MonoBehaviour
{
    public bool isPowered = false;

    public void PowerOn()
    {
        if (isPowered) return;

        isPowered = true;
        Debug.Log("Socket ON");

        // nanti bisa:
        // buka pintu
        // nyalakan lampu
        // aktifkan mesin
    }

    public void ResetPower()
    {
        isPowered = false;
    }
}