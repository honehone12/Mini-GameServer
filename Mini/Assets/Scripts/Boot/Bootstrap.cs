using UnityEngine;
using UnityEngine.Events;
using Unity.Netcode;

namespace Mini
{
    [RequireComponent(typeof(NetworkManager))]
    public class Bootstrap : SessionedBehaviour
    {
        public static Bootstrap Singleton
        {
            get; private set;
        }

        public static string UserUUID
        {
            get; private set;
        }

        public static string OneTimeID
        {
            get; private set;
        }

        [SerializeField]
        BootType bootType;
        [Space]
        public UnityEvent OnServerStarted = new();
        public UnityEvent OnClientStarted = new();

        public BootType BootType => bootType;

        void Awake()
        {
            if (Singleton == null)
            {
                Singleton = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

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
                    OnClientStarted.Invoke();
                    break;
                case BootType.UnauthorizedHost:
                    NetworkManager.Singleton.StartHost();
                    OnServerStarted.Invoke();
                    OnClientStarted.Invoke();
                    break;
                case BootType.Client:
                case BootType.Host:
                    base.Start();
                    break;
                default:
                    throw new System.Exception("unexpected boot type");
            }
        }

        protected override void OnSuccessfullyAuthorized(string uuid, string oneTimeId)
        {
            switch (bootType)
            {
                case BootType.Client:
                    UserUUID = uuid;
                    OneTimeID = oneTimeId;
                    NetworkManager.Singleton.StartClient();
                    OnClientStarted.Invoke();
                    break;
                case BootType.Host:
                    UserUUID = uuid;
                    OneTimeID = oneTimeId;
                    NetworkManager.Singleton.StartHost();
                    OnServerStarted.Invoke();
                    OnClientStarted.Invoke();
                    break;
                default:
                    throw new System.Exception("unexpected boot type");
            }
        }
    }
}
