using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Mini
{
    public class LoginManager : AuthHttpClientBehaviour
    {
        Coroutine loginCoroutine;

        public void OnSignUpButtonClicked()
        {
            _ = StartCoroutine(LoadRegisterScene());
        }

        IEnumerator LoadRegisterScene()
        {
            yield return SceneManager.LoadSceneAsync("Register");
        }

        public void OnLoginButtonClicked()
        {
            loginCoroutine = StartCoroutine(SendLoginRequest());
        }

        IEnumerator SendLoginRequest()
        {
            var form = new WWWForm();
            form.AddField("email", emailTextBuffer);
            form.AddField("password", passwordTextBuffer);
            using var req = UnityWebRequest.Post(RequestUrl("/login"), form);
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                requestOKPanel.SetActive(true);
                yield return new WaitForSeconds(0.5f);
                yield return SceneManager.LoadSceneAsync("Dev");
            }
            else
            {
                Debug.LogError(req.error);
            }
        }
    }
}
