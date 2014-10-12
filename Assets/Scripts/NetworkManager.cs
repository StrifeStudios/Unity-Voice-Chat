using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(NetworkView))]
public class NetworkManager : Singleton<NetworkManager>
{
    private string gameTypeName = "Voice Chat";
    private bool refreshing = false;
    private HostData[] hostData;

    public static bool gameStarted = false;

    public int PlayerNumber = 0;

    private NetworkView nv;

    //GUI Variables
    private string gameName = "Default Game Name";
    private Vector2 scrollPosition;

    private void Start()
    {
        nv = GetComponent<NetworkView>();
        nv.stateSynchronization = NetworkStateSynchronization.Off;
        scrollPosition = new Vector2();

        refreshHostList();
    }

    private void Update()
    {
        if (refreshing)
        {
            if (MasterServer.PollHostList().Length > 0)
            {
                refreshing = false;
                Debug.Log("HostList Length: " + MasterServer.PollHostList().Length);
                hostData = MasterServer.PollHostList();
            }
        }
    }

    private void startServer()
    {
        Debug.Log("startServer called");

        bool useNAT = !Network.HavePublicAddress();
        Network.InitializeServer(32, 25001, useNAT);
        MasterServer.RegisterHost(gameTypeName, gameName);
        PlayerNumber = 0;
    }

    private void refreshHostList()
    {
        MasterServer.RequestHostList(gameTypeName);
        refreshing = true;
    }

    private void PrintHostData()
    {
        HostData[] hostData = MasterServer.PollHostList();
        Debug.Log("HostData length " + hostData.Length);
    }

    #region Messages
    private void OnServerInitialized()
    {
        Debug.Log("Server initialized");
        gameStarted = true;
    }

    private void OnConnectedToServer()
    {
        gameStarted = true;
    }

    private void OnMasterServerEvent(MasterServerEvent mse)
    {
        Debug.Log("Master Server Event: " + mse.ToString());
    }

    private void OnPlayerConnected(NetworkPlayer player)
    {
        Debug.Log("OnPlayerConnected, playerID:" + player.ToString());
        Debug.Log("Player Count : " + Network.connections.Length);
    }

    void OnDisconnectedFromServer(NetworkDisconnection info)
    {
        gameStarted = false;

        if (Network.isServer)
            Debug.Log("Local server connection disconnected");
        else
            if (info == NetworkDisconnection.LostConnection)
                Debug.Log("Lost connection to the server");
            else
                Debug.Log("Successfully diconnected from the server");
    }
    #endregion

    #region GUI
    private void OnGUI()
    {
        if (!Network.isClient && !Network.isServer)
        {

            //Setup the GUIStyles
            GUI.skin.button.normal.textColor = Color.white;
            GUI.skin.button.fontSize = (int)(15 * Util.GetScale());
            GUI.skin.textField.normal.textColor = Color.white;
            GUI.skin.textField.fontSize = (int)(12 * Util.GetScale());
            GUI.skin.label.normal.textColor = Color.black;
            GUI.skin.label.fontSize = (int)(15 * Util.GetScale());

            DrawTitle();

            GUILayout.BeginArea(new Rect(0f, Screen.height * 0.33f, Screen.width, Screen.height * 0.6f));
            GUILayout.BeginHorizontal();
            GUILayout.Space(Screen.width * 0.265f);

            //First vertical contains menu buttons
            GUILayout.BeginVertical();
            if (GUILayout.Button("Start Server",
                                 GUILayout.Width(125f * Util.GetScale()),
                                 GUILayout.Height(50f * Util.GetScale())))
            {
                Debug.Log("Starting Server");
                startServer();
            }

            GUILayout.Space(10f * Util.GetScale());

            if (GUILayout.Button("Refresh Hosts",
                                 GUILayout.Width(125f * Util.GetScale()),
                                 GUILayout.Height(50f * Util.GetScale())))
            {
                Debug.Log("Refreshing Hosts");
                refreshHostList();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();

            GUILayout.Space(10f * Util.GetScale());

            //Second vertical contains host data
            GUILayout.BeginVertical(GUILayout.Width(225f));
            GUILayout.Label("Server Name:",
                            GUILayout.Width(200f * Util.GetScale()),
                            GUILayout.Height(25f * Util.GetScale()));
            gameName = GUILayout.TextField(gameName,
                                           GUILayout.Width(200f * Util.GetScale()),
                                           GUILayout.Height(25f * Util.GetScale()));
            GUILayout.Space(10f * Util.GetScale());
            GUILayout.Label("Select a Game to Join:",
                            GUILayout.Width(200f * Util.GetScale()),
                            GUILayout.Height(25f * Util.GetScale()));

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            if (hostData != null)
            {
                foreach (HostData hd in hostData)
                {
                    if (GUILayout.Button(hd.gameName,
                                         GUILayout.Width(200f * Util.GetScale()),
                                         GUILayout.Height(25f * Util.GetScale())))
                    {
                        Debug.Log("Connecting to server");
                        Network.Connect(hd);
                    }
                    GUILayout.Space(10f * Util.GetScale());
                }
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
        else if (!gameStarted)
        {
            GUIStyle textStyle = new GUIStyle();
            textStyle.normal.textColor = Color.black;
            textStyle.fontSize = (int)(25 * Util.GetScale());

            DrawTitle();

            GUILayout.BeginArea(new Rect(0f, 0f, Screen.width, Screen.height));
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Waiting for another user to connect...", textStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
    }

    private void DrawTitle()
    {
        GUIStyle titleStyle = new GUIStyle();
        titleStyle.normal.textColor = Color.black;
        titleStyle.fontSize = (int)(75 * Util.GetScale());

        GUILayout.BeginArea(new Rect(0f, 0f, Screen.width, Screen.height * 0.25f));
        GUILayout.BeginVertical();
        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Iron Strife Voice Chat", titleStyle);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
    #endregion
}