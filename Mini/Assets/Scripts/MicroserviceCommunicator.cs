using UnityEngine;
using UnityEngine.Networking;

namespace Mini
{
    [CreateAssetMenu(menuName = "MicroserviceCommunicator")]
    public class MicroserviceCommunicator : ScriptableObject
    {
        [SerializeField]
        string protocol = "http://";
        [SerializeField]
        string ipAddr = "0.0.0.0";
        [SerializeField]
        string port;

        string RequestURL(string routeWithSlash)
        {
            return protocol + ipAddr + ":" + port + routeWithSlash;
        }

        public UnityWebRequest HttpRequest(string routeWithSlash, WWWForm form)
        {
            return UnityWebRequest.Post(RequestURL(routeWithSlash), form);
        }
    }
}
