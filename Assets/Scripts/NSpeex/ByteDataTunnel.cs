using System;
using System.Collections.Generic;

/// <summary>
/// A component that simply sends byte arrays over the network.
/// </summary>
using UnityEngine;
public class ByteDataTunnel : MonoBehaviour
{
    public event Action<Byte[], int> DataReceived;

    [SerializeField]
    private List<NetworkPlayer> remoteTargets;
    public List<NetworkPlayer> RemoteTargets
    {
        get { return remoteTargets; }
        set { remoteTargets = value; }
    }

    public void SendDataToRemote(Byte[] bytes, int numBytes)
    {
        foreach (NetworkPlayer remoteTarget in this.remoteTargets)
        {
            if (remoteTarget.ipAddress == "unassigned")
            {
                throw new InvalidOperationException("No remote target set for audio tunnel.");
            }

            networkView.RPC("ReadByteData_Remote", remoteTarget, bytes, numBytes);
        }
    }

    [RPC]
    public void ReadByteData_Remote(byte[] data, int numBytes)
    {
        OnDataReceived(data, numBytes);
    }

    private void OnDataReceived(byte[] data, int numBytes)
    {
        if (DataReceived != null)
        {
            DataReceived(data, numBytes);
        }
    }
}