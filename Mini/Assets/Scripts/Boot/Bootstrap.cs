using UnityEngine;
using Unity.Netcode;

namespace Mini
{
    public enum BootType : byte
    {
        Server,
        Client,
        Host,
        Dev
    }

    [RequireComponent(typeof(NetworkManager))]
    public class Bootstrap : SessionedBehaviour
    {
        [SerializeField]
        BootType bootType;

        protected override void OnSuccessfullyAuthorized(string uuid)
        {
            switch (bootType)
            {
                case BootType.Server:
                    NetworkManager.Singleton.StartServer();
                    break;
                case BootType.Client:
                    NetworkManager.Singleton.StartClient();
                    break;
                case BootType.Host:
                    NetworkManager.Singleton.StartHost();
                    break;
                case BootType.Dev:
                    Debug.Log("OkaeriNasaimase, goshyujinsama.");
                    break;
                default:
                    throw new System.Exception("unexpected boot type");
            }
        }
    }
}
