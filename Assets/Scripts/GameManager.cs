using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : MonoBehaviour
{
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
         SubmitNewPosition();
      }
      
      GUILayout.EndArea();
   }

   private static void StartButtons()
   {
      if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();
      if (GUILayout.Button("Client")) NetworkManager.Singleton.StartClient();
      if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();
   }

   private static void StatusLabels()
   {
      var mode = NetworkManager.Singleton.IsHost ? "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";
      
      GUILayout.Label("Transport: " + NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
      GUILayout.Label("Mode: " + mode);
   }

   private static void SubmitNewPosition()
   {
      if (GUILayout.Button(NetworkManager.Singleton.IsServer ? "Move" : "Request Position Change"))
      {
         if (NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsClient)
         {
            foreach (var uid in NetworkManager.Singleton.ConnectedClientsIds)
            {
               var networkPlayer = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(uid).GetComponent<Player>();
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
}