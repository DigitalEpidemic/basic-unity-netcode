using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEngine;

public class GameManager : MonoBehaviour
{
   private static string _playerName = "";
   private static Dictionary<ulong, string> _clientData;

   public static string GetPlayerName(ulong clientId)
   {
      return _clientData.TryGetValue(clientId, out string playerName) ? playerName : null;
   }

   private void Start()
   {
      NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
   }

   private void OnGUI()
   {
      GUILayout.BeginArea(new Rect(10, 10, 300, 300));

      if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
      {
         StartButtons();
      }
      else
      {
         StatusLabels();
         SendPlayerMove();
         SendPlayerFire();
      }
      
      GUILayout.EndArea();
   }

   private static void StartButtons()
   {
      if (GUILayout.Button("Host")) StartHost();
      if (GUILayout.Button("Client")) StartClient();
      if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();
      _playerName = GUILayout.TextField(_playerName, 25);
   }

   private static void StartHost()
   {
      _clientData = new Dictionary<ulong, string>();
      _clientData[NetworkManager.Singleton.LocalClientId] = _playerName;
      
      NetworkManager.Singleton.NetworkConfig.ConnectionData = Encoding.ASCII.GetBytes(_playerName);
      NetworkManager.Singleton.StartHost();
   }

   private static void StartClient()
   {
      if (NetworkManager.Singleton.IsServer)
      {
         _clientData[NetworkManager.Singleton.LocalClientId] = _playerName;
      }
      NetworkManager.Singleton.NetworkConfig.ConnectionData = Encoding.ASCII.GetBytes(_playerName);
      NetworkManager.Singleton.StartClient();
   }
   
   private static void StatusLabels()
   {
      var mode = NetworkManager.Singleton.IsHost ? "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";
      
      GUILayout.Label("Transport: " + NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
      GUILayout.Label("Mode: " + mode);
   }

   private static void SendPlayerMove()
   {
      if (GUILayout.Button(NetworkManager.Singleton.IsServer ? "Move" : "Request Position Change"))
      {
         if (NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsClient)
         {
            foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
               var networkPlayer = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientId).GetComponent<Player>();
               networkPlayer.Move();
            }
         }
         else
         {
            var localPlayer = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().GetComponent<Player>();
            localPlayer.Move();
         }
      }
   }

   private static void SendPlayerFire()
   {
      if (GUILayout.Button(NetworkManager.Singleton.IsServer ? "Fire" : "Request Fire"))
      {
         if (NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsClient)
         {
            foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
               var networkPlayer = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientId).GetComponent<Player>();
               networkPlayer.Fire();
            }
         }
         else
         {
            var localPlayer = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().GetComponent<Player>();
            localPlayer.Fire();
         }
      }
   }

   private static void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
   {
      string payload = Encoding.ASCII.GetString(request.Payload);
      
      _clientData[request.ClientNetworkId] = payload;
      
      response.Approved = true;
      response.CreatePlayerObject = true;
      response.Pending = false;
   }
}
