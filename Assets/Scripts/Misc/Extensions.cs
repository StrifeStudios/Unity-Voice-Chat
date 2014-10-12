using UnityEngine;

/// <summary>
/// Useful extension methods
/// </summary>
public static class Extensions
{
    public static void RpcToServer(this Component mb, string methodName, params object[] args)
    {
        mb.networkView.RPC(methodName, RPCMode.Server, args);
    }
}