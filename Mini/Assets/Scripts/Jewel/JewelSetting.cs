using UnityEngine;

namespace Mini
{
    [System.Serializable]
    public class JewelSetting
    {
        public Material material;
        public ColorCode colorCode = ColorCode.NotSelected;
        public int incrementOnCollect = 1;
    }
}
