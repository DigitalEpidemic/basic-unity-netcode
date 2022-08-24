using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RpcTest : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            TestServerRpc(0);
        }
    }

    [ClientRpc]
    private void TestClientRpc(int value)
    {
        Debug.Log("Client received the RPC value: " + value);
        TestServerRpc(value + 1);
    }

    [ServerRpc]
    private void TestServerRpc(int value)
    {
        Debug.Log("Server received the RPC value: " + value);
        TestClientRpc(value);
    }
}
