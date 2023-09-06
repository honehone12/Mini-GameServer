using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Mini
{
    [CreateAssetMenu(menuName = "JewelMaterialList")]
    public class JewelMaterialList : ScriptableObject
    {
        [SerializeField]
        List<JewelMaterial> materialList = new();

        public int Count
        {
            get
            {
                var len = materialList.Count;
                Assert.IsTrue(len > 0);
                return len;
            }
        }

        public JewelMaterial Get(int index)
        {
            var len = materialList.Count;
            Assert.IsTrue(len > 0);
            Assert.IsTrue(index > 0 && index < len);
            return materialList[index];
        }

        public JewelMaterial Random()
        {
            var len = materialList.Count;
            Assert.IsTrue(len > 0);
            var idx = UnityEngine.Random.Range(0, len);
            return materialList[idx];
        }
    }
}
