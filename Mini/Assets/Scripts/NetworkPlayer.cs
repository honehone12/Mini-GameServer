using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Assertions;
using Unity.Netcode;

namespace Mini
{
    public class NetworkPlayer : NetworkBehaviour
    {
        [SerializeField]
        MicroserviceCommunicator communicator;

        void Awake()
        {
            Assert.IsNotNull(communicator);
        }

        public override void OnNetworkSpawn()
        {
            if (IsClient && IsOwner)
            {
                var bootType = Bootstrap.Singleton.BootType;
                if (bootType == BootType.Client || bootType == BootType.Host)
                {
                    IntroduceMySelfServerRpc(Bootstrap.UserUUID, Bootstrap.OneTimeID);
                }
            }
        }

        [ServerRpc(RequireOwnership = true, Delivery = RpcDelivery.Reliable)]
        public void IntroduceMySelfServerRpc(string playerUuid, string oneTimeId, ServerRpcParams serverRpcParams = default)
        {
            _ = StartCoroutine(VerifyOneTimeId(playerUuid, oneTimeId, serverRpcParams.Receive.SenderClientId));

            // here also can be used for name etc...and share between players with NetworkVariable
        }

        IEnumerator VerifyOneTimeId(string playerUuid, string oneTimeId, ulong netcodeID)
        {
            var form = new WWWForm();
            form.AddField("uuid", playerUuid);
            form.AddField("id", oneTimeId);
            using var req = communicator.HttpRequest("/session/verify", form);
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
            
            }
            else
            {
                Debug.LogWarning(req.error);
                NetworkManager.Singleton.DisconnectClient(netcodeID);
            }
        }
    }
}


