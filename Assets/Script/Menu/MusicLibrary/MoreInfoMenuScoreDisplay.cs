using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using YARG.Core;
using YARG.Core.Input;
using YARG.Core.Song;
using YARG.Core.Utility;
using YARG.Helpers.Extensions;
using YARG.Menu.Navigation;
using YARG.Menu.Persistent;
using YARG.Scores;
using YARG.Song;
using static System.Globalization.CultureInfo;

namespace YARG.Menu.MusicLibrary
{
    public class MoreInfoMenuScoreDisplay : MonoBehaviour
    {
        [SerializeField]
        private Image _difficultyIcon;
        [SerializeField]
        private TextMeshProUGUI _playerName;
        [SerializeField]
        private TextMeshProUGUI _scoreText;
        [SerializeField]
        private TextMeshProUGUI _percentText;
        [SerializeField]
        private StarView _starView;

    }
}
