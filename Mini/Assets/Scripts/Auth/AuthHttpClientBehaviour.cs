using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Mini
{
    public class AuthHttpClientBehaviour : AuthHttpClientBase
    {
        [SerializeField]
        protected Toggle showPassword;
        [SerializeField]
        protected TMP_InputField passwordInputField;
        [Space]
        [SerializeField]
        protected GameObject requestOKPanel;

        protected string emailTextBuffer;
        protected string passwordTextBuffer;

        protected override void Awake()
        {
            base.Awake();
            Assert.IsNotNull(showPassword);
            Assert.IsNotNull(passwordInputField);
            Assert.IsNotNull(requestOKPanel);
            showPassword.isOn = false;
            passwordInputField.contentType = TMP_InputField.ContentType.Password;
            requestOKPanel.SetActive(false);
        }

        public void OnEmailTextEdited(string edited)
        {
            emailTextBuffer = edited;
        }

        public void OnPasswordTextEdited(string edited)
        {
            passwordTextBuffer = edited;
        }

        public void OnShowPassword(bool show)
        {
            passwordInputField.contentType = show ?
                TMP_InputField.ContentType.Standard : TMP_InputField.ContentType.Password;
            if (show)
            {
                passwordInputField.textComponent.text = passwordTextBuffer;
            }
        }
    }
}
