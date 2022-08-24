using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : NetworkBehaviour
{
    public NetworkVariable<Vector3> position = new();

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            Move();
        }
    }
    
    private void Update()
    {
        transform.position = position.Value;
    }
    
    public void Move()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            position.Value = GetRandomPositionOnPlane();
        }
        else
        {
            SubmitPositionRequestServerRpc();
        }
    }
    
    private static Vector3 GetRandomPositionOnPlane()
    {
        return new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(-3f, 3f));
    }

    [ServerRpc]
    private void SubmitPositionRequestServerRpc(ServerRpcParams rpcParams = default)
    {
        position.Value = GetRandomPositionOnPlane();
    }
}
