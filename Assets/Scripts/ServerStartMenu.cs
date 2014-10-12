using UnityEngine;
using System.Collections;

public class ServerStartMenu : MonoBehaviour
{
    [SerializeField]
    private Rect drawArea = new Rect(500, 0, 300, 1000);
    private int listenPort = 21000;
    [SerializeField]
    private int maxConnections = 32;
    void OnGUI()
    {
        GUILayout.BeginArea(drawArea);
        GUILayout.BeginVertical();
        GUILayout.Label("Status: " + (Network.isServer ? "Server" : Network.isClient ? "Client" : "Disconnected"));
        if (Network.isServer)
        {
            GUILayout.Label("Listen Port: " + this.listenPort);
        }
        if (!Network.isServer && !Network.isClient)
        {
            GUILayout.Label("Listen port:");
            string newPort = GUILayout.TextField(listenPort.ToString());
            int.TryParse(newPort, out listenPort);
            if (GUILayout.Button("Host"))
            {
                HostButtonPressed();
            }
        }
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    private void HostButtonPressed()
    {
        Network.InitializeServer(this.maxConnections, this.listenPort, false);
    }
}