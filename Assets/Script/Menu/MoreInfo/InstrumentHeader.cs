using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using YARG.Core;
using YARG.Core.Input;
using YARG.Core.Logging;
using YARG.Core.Song;
using YARG.Menu;
using YARG.Menu.MusicLibrary;
using YARG.Menu.Navigation;
using YARG.Scores;
using YARG.Song;

namespace YARG.Menu.MoreInfo
{
    public class InstrumentHeader : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _text;

        public void SetText(string text)
        {
            _text.text = text;
        }

        public void SetSelected(bool selected)
        {
            if (selected)
            {
                _text.fontSize = 32;
                _text.alpha = 1f;
            }
            else
            {
                _text.fontSize = 24;
                _text.alpha = 0.2f;
            }
        }
    }
}