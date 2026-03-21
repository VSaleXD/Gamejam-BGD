using UnityEngine;
using System.Collections.Generic;

public class SocketColorRandomizer : MonoBehaviour
{
    private void Start()
    {
        PowerSocket[] sockets = FindObjectsByType<PowerSocket>(FindObjectsSortMode.None);

        List<PowerSocket> randomSockets = new List<PowerSocket>();

        foreach (var s in sockets)
        {
            // ⭐ hanya yang random mode
            if (s.UseRandomColor())
                randomSockets.Add(s);
        }

        List<PowerSocket.SocketColor> colors = new List<PowerSocket.SocketColor>()
        {
            PowerSocket.SocketColor.Red,
            PowerSocket.SocketColor.Blue,
            PowerSocket.SocketColor.Green,
            PowerSocket.SocketColor.Yellow
        };

        Shuffle(colors);

        for (int i = 0; i < randomSockets.Count; i++)
        {
            randomSockets[i].SetColor(colors[i]);
        }
    }

    void Shuffle(List<PowerSocket.SocketColor> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            var temp = list[i];
            list[i] = list[rand];
            list[rand] = temp;
        }
    }
}