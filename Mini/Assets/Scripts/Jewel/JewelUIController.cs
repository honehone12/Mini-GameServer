using UnityEngine;
using UnityEngine.Assertions;
using TMPro;

namespace Mini
{
    public class JewelUIController : MonoBehaviour
    {
        [SerializeField]
        TMP_Text redNumberText;
        [SerializeField]
        TMP_Text blueNumberText;
        [SerializeField]
        TMP_Text greenNumberText;
        [SerializeField]
        TMP_Text yellowNumberText;
        [SerializeField]
        TMP_Text blackNumberText;

        public static JewelUIController Singleton
        {
            get; private set;
        }

        void Awake()
        {
            Assert.IsNotNull(redNumberText);
            Assert.IsNotNull(blueNumberText);
            Assert.IsNotNull(greenNumberText);
            Assert.IsNotNull(yellowNumberText);
            Assert.IsNotNull(blackNumberText);

            if (Singleton == null)
            {
                Singleton = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void UpdateJewelNumbers(JewelModel jewels)
        {
            redNumberText.text = jewels.Red.ToString();
            blueNumberText.text = jewels.Blue.ToString();
            greenNumberText.text = jewels.Green.ToString();
            yellowNumberText.text = jewels.Yellow.ToString();
            blackNumberText.text = jewels.Black.ToString();
        }
    }
}
