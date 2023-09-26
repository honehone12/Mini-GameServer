using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Assertions;
using Unity.Netcode;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Unity.Netcode.Transports.UTP;

namespace Mini
{
    [RequireComponent(typeof(NetworkManager))]
    public class Bootstrap : SessionedBehaviour
    {
        [System.Serializable]
        public class ZoneResponse
        {
            public string Address;
            public string Port;
        }

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

        [Header("Zone")]
        [SerializeField]
        string zoneAddress;
        [SerializeField]
        string zonePort;
        [SerializeField]
        string zoneRouteName;

        protected string ZoneRequestUrl(string routeWithSlash)
        {
            return (Debug.isDebugBuild ? "http://" : "https://") + zoneAddress + ":" + zonePort + routeWithSlash;
        }

        public BootType BootType => bootType;

        protected override void Awake()
        {
            base.Awake();
            Assert.IsFalse(string.IsNullOrEmpty(zoneAddress));
            Assert.IsFalse(string.IsNullOrEmpty(zonePort));
            Assert.IsFalse(string.IsNullOrEmpty(zoneRouteName));

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

        IEnumerator Boot(string uuid, string oneTimeId)
        {
            using var req = UnityWebRequest.Get(ZoneRequestUrl("/" + zoneRouteName));
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                var res = JsonUtility.FromJson<ZoneResponse>(req.downloadHandler.text);
                if (TryGetComponent<UnityTransport>(out var transport))
                {
                    if (ushort.TryParse(res.Port, out var p))
                    {
                        switch (bootType)
                        {
                            case BootType.Client:
                                transport.SetConnectionData(res.Address, p);

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
                    else
                    {
                        Debug.LogError("received unexpected response");
                        yield return SceneManager.LoadSceneAsync("Login");
                    }
                }
                else
                {
                    Debug.LogError("could not find transport component");
                    yield return SceneManager.LoadSceneAsync("Login");
                }
            }
            else
            {
                Debug.LogError(req.error);
                yield return SceneManager.LoadSceneAsync("Login");
            }
        }

        protected override void OnSuccessfullyAuthorized(string uuid, string oneTimeId)
        {
            _ = StartCoroutine(Boot(uuid, oneTimeId));
        }
    }
}
