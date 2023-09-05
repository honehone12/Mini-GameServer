using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Mini
{
    public class HttpClientBehaviour : MonoBehaviour
    {
        [SerializeField]
        protected string uri;
        [SerializeField]
        protected string port;

        protected string RequestUrl(string routeWithSlash)
        {
            return (Debug.isDebugBuild ? "http://" : "https://") + uri + ":" + port + routeWithSlash;
        }
    }
}
