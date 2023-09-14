using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Assertions;
using Unity.Netcode;
using UnityEngine.Events;

namespace Mini
{
    public class NetworkPlayer : NetworkBehaviour
    {
        [SerializeField]
        MicroserviceCommunicator communicator;
        [Space]
        public UnityEvent OnOneTimeIdVerified = new();

        string userUuidServerCache;

        void Awake()
        {
            Assert.IsNotNull(communicator);
        }

        public bool TryGetUserUuidServerCache(out string uuid)
        {
            uuid = userUuidServerCache;
            return string.IsNullOrEmpty(userUuidServerCache);
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

        [ClientRpc(Delivery = RpcDelivery.Reliable)]
        public void ResponseOneTimeIdVerifiedClientRpc(ClientRpcParams rpcParams = default)
        {
            OnOneTimeIdVerified.Invoke();
        }

        [ServerRpc(RequireOwnership = true, Delivery = RpcDelivery.Reliable)]
        public void IntroduceMySelfServerRpc(string playerUuid, string oneTimeId, ServerRpcParams rpcParams = default)
        {
            userUuidServerCache = playerUuid;
            _ = StartCoroutine(VerifyOneTimeId(playerUuid, oneTimeId, rpcParams.Receive.SenderClientId));

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
                ResponseOneTimeIdVerifiedClientRpc(new ClientRpcParams
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds = new ulong[] { netcodeID }
                    }
                });
            }
            else
            {
                Debug.LogWarning(req.error);
                NetworkManager.Singleton.DisconnectClient(netcodeID);
            }
        }
    }
}


