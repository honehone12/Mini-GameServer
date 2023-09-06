using UnityEngine;
using UnityEngine.Events;
using Unity.Netcode;

namespace Mini
{
    [RequireComponent(typeof(NetworkManager))]
    public class Bootstrap : SessionedBehaviour
    {
        [SerializeField]
        BootType bootType;
        [Space]
        public UnityEvent OnServerStarted = new();
        public UnityEvent<Player> OnClientStarted = new();

        protected override void Start()
        {
            switch (bootType)
            {
                case BootType.Server:
                    NetworkManager.Singleton.StartServer();
                    OnServerStarted.Invoke();
                    break;
                case BootType.UnauthorizedClient:
                    NetworkManager.Singleton.StartClient();
                    OnClientStarted.Invoke(new Player
                    {
                        playerType = BootType.UnauthorizedClient,
                        uuid = string.Empty
                    });
                    break;
                case BootType.UnauthorizedHost:
                    NetworkManager.Singleton.StartHost();
                    OnServerStarted.Invoke();
                    OnClientStarted.Invoke(new Player
                    {
                        playerType = BootType.UnauthorizedHost,
                        uuid = string.Empty
                    });
                    break;
                case BootType.Client:
                case BootType.Host:
                    base.Start();
                    break;
                default:
                    throw new System.Exception("unexpected boot type");
            }
        }

        protected override void OnSuccessfullyAuthorized(string userUuid, string sessionId)
        {
            switch (bootType)
            {
                case BootType.Client:
                    NetworkManager.Singleton.StartClient();
                    OnClientStarted.Invoke(new Player
                    {
                        playerType = BootType.Client,
                        uuid = userUuid
                    });
                    break;
                case BootType.Host:
                    NetworkManager.Singleton.StartHost();
                    OnServerStarted.Invoke();
                    OnClientStarted.Invoke(new Player
                    {
                        playerType = BootType.Host,
                        uuid = userUuid
                    });
                    break;
                default:
                    throw new System.Exception("unexpected boot type");
            }
        }

        // This RPC is still self-declared.
        // Or is it bettter to send raw cookie ??
        [ServerRpc]
        public void IntroduceMySelfServerRpc(BootType playerType, string playerUuid, string sessionId)
        {

        }
    }
}
