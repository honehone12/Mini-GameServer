using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace Mini
{
    public class AuthHttpClientBase : MonoBehaviour
    {
        [Header("Auth")]
        [SerializeField]
        protected string authAddress;
        [SerializeField]
        protected string authPort;

        protected virtual void Awake()
        {
            Assert.IsFalse(string.IsNullOrEmpty(authAddress));
            Assert.IsFalse(string.IsNullOrEmpty(authPort));
        }

        protected string AuthRequestUrl(string routeWithSlash)
        {
            return (Debug.isDebugBuild ? "http://" : "https://") + authAddress + ":" + authPort + routeWithSlash;
        }
    }
}
