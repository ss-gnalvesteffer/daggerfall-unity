using System;
using DaggerfallWorkshop.Game;
using UnityEngine;

namespace DaggerfallWorkshop
{
    public class DaggerfallBed : MonoBehaviour
    {
        void Start()
        {
        }

        internal void Rest()
        {
            DaggerfallUI.PostMessage(DaggerfallUIMessages.dfuiOpenRestWindow);
        }
    }
}
