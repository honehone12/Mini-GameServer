using System.Collections;
using System.Drawing;
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

        JewelModel jewelDataCache = new();

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
                    IncrementJewelServerRpc(Bootstrap.UserUUID, color, incr);
                }
            }
        }

        public void SyncJewelData()
        {
            if (IsClient && IsOwner)
            {
                var bootType = Bootstrap.Singleton.BootType;
                if (bootType == BootType.Client || bootType == BootType.Host)
                {
                    RequestJewelDataServerRpc(Bootstrap.UserUUID);
                }
            }
        }

        [ClientRpc(Delivery = RpcDelivery.Reliable)]
        public void ResponseJewelDataClientRpc(uint red, uint blue, uint green, uint yellow, uint black, ClientRpcParams rpcParams = default)
        {
            jewelDataCache = new JewelModel
            {
                Red = red,
                Blue = blue,
                Green = green,
                Yellow = yellow,
                Black = black
            };
            JewelUIController.Singleton.UpdateJewelNumbers(jewelDataCache);
        }

        [ServerRpc(RequireOwnership = true, Delivery = RpcDelivery.Reliable)]
        public void RequestJewelDataServerRpc(string uuid, ServerRpcParams rpcParams = default)
        {
            _ = StartCoroutine(GetAll(uuid, rpcParams.Receive.SenderClientId));
        }

        [ServerRpc(RequireOwnership = true, Delivery = RpcDelivery.Reliable)]
        public void IncrementJewelServerRpc(string uuid, ColorCode color, int incr, ServerRpcParams rpcParams = default)
        {
            _ = StartCoroutine(IncrJewel(uuid, color, incr, rpcParams.Receive.SenderClientId));
        }

        IEnumerator IncrJewel(string uuid, ColorCode color, int incr, ulong netcodeId)
        {
            var form = new WWWForm();
            form.AddField("uuid", uuid);
            form.AddField("color", (int)color);
            form.AddField("incr", incr);
            using var req = communicator.HttpRequest("/jewel/incr", form);
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                // well, actually don't have to get all
                _ = StartCoroutine(GetAll(uuid, netcodeId));
            }
            else
            {
                Debug.LogError(req.error);
                // want backup request
            }
        }

        IEnumerator GetAll(string uuid, ulong netcodeId)
        {
            var form = new WWWForm();
            form.AddField("uuid", uuid);
            using var req = communicator.HttpRequest("/jewel/get-all", form);
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                var res = JsonUtility.FromJson<JewelModel>(req.downloadHandler.text);
                jewelDataCache = res;
                ResponseJewelDataClientRpc(
                    res.Red, res.Blue, res.Green, res.Yellow, res.Black,
                    new ClientRpcParams
                    {
                        Send = new ClientRpcSendParams
                        {
                            TargetClientIds = new ulong[] { netcodeId }
                        }
                    }
                );
            }
            else
            {
                Debug.LogError(req.error);
            }
        }
    }
}


