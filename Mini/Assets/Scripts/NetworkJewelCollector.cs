using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;

namespace Mini
{
    public class NetworkJewelCollector : NetworkBehaviour
    {
        [SerializeField]
        MicroserviceCommunicator communicator;

        void Awake()
        {
            Assert.IsNotNull(communicator);
        }

        public void CollectJewel(ColorCode color, int incr)
        {
            if (IsClient && IsOwner)
            {
                var bootType = Bootstrap.Singleton.BootType;
                if (bootType == BootType.Client || bootType == BootType.Host)
                {
                    IncrementJewelServerRpc(color, incr);
                }
            }
        }

        [ServerRpc(RequireOwnership = true, Delivery = RpcDelivery.Reliable)]
        public void IncrementJewelServerRpc(ColorCode color, int incr)
        {
            _ = StartCoroutine(IncrJewel(color, incr));
        }

        IEnumerator IncrJewel(ColorCode color, int incr)
        {
            var form = new WWWForm();
            form.AddField("uuid", Bootstrap.UserUUID);
            form.AddField("color", (int)color);
            form.AddField("incr", incr);
            using var req = communicator.HttpRequest("/jewel/incr", form);
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {

            }
            else
            {
                Debug.LogError(req.error);
                // want backup request
            }
        }
    }
}


