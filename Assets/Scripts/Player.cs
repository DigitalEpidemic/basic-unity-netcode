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
    
    private static Vector3 GetRandomPositionOnPlane()
    {
        return new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(-3f, 3f));
    }


    private void Move()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            var randomPosition = GetRandomPositionOnPlane();
            transform.position = randomPosition;
            position.Value = randomPosition;
        }
        else
        {
            SubmitPositionRequestServerRpc();
        }
    }

    [ServerRpc]
    private void SubmitPositionRequestServerRpc(ServerRpcParams rpcParams = default)
    {
        position.Value = GetRandomPositionOnPlane();
    }
}
