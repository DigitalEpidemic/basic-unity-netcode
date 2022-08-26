using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : NetworkBehaviour
{
    public NetworkVariable<Vector3> position = new();
    
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private Transform _spawnPointTransform;
    
    private GameObject _projectileGameObject;
    private NetworkObject _projectileNetworkObject;

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
    
    [ServerRpc]
    private void SubmitPositionRequestServerRpc(ServerRpcParams rpcParams = default)
    {
        position.Value = GetRandomPositionOnPlane();
    }

    public void Fire()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            SpawnProjectile();
        }
        else
        {
            SubmitFireRequestServerRpc();
        }
    }
    
    [ServerRpc]
    private void SubmitFireRequestServerRpc(ServerRpcParams rpcParams = default)
    {
        SpawnProjectile();
    }

    private void SpawnProjectile()
    {
        _projectileGameObject = Instantiate(_projectilePrefab, _spawnPointTransform.position, Quaternion.identity);

        _projectileNetworkObject = _projectileGameObject.GetComponent<NetworkObject>();
        _projectileNetworkObject.Spawn();
    }
    
    private static Vector3 GetRandomPositionOnPlane()
    {
        return new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(-3f, 3f));
    }
}
