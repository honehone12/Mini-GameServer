using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Mini
{
    [CreateAssetMenu(menuName = "JewelSettingList")]
    public class JewelSettingList : ScriptableObject
    {
        [SerializeField]
        List<JewelSetting> settingList = new();

        public int Count
        {
            get
            {
                var len = settingList.Count;
                Assert.IsTrue(len > 0);
                return len;
            }
        }

        public JewelSetting Get(int index)
        {
            var len = settingList.Count;
            Assert.IsTrue(len > 0);
            Assert.IsTrue(index > 0 && index < len);
            return settingList[index];
        }

        public JewelSetting Random()
        {
            var len = settingList.Count;
            Assert.IsTrue(len > 0);
            var idx = UnityEngine.Random.Range(0, len);
            return settingList[idx];
        }
    }
}
