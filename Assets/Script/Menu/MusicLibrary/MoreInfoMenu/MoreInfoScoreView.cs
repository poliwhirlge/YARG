using System;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YARG.Core.Input;
using YARG.Core.Logging;
using YARG.Core.Song;
using YARG.Helpers.Extensions;
using YARG.Menu;
using YARG.Menu.MusicLibrary;
using YARG.Menu.Navigation;
using YARG.Menu.Persistent;
using YARG.Scores;
using YARG.Song;

namespace YARG.Menu.MusicLibrary
{
    public class MoreInfoScoreView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _profileName;
        [SerializeField]
        private TextMeshProUGUI _scoreText;
        [SerializeField]
        private TextMeshProUGUI _percentText;
        [SerializeField]
        private StarView _starView;

        internal void SetScore(PlayerScoreRecord score)
        {
            _profileName.text = score.PlayerId.ToString();
            _scoreText.text = score.Score.ToString();
            _percentText.text = $"{Math.Floor((float) score.Percent * 100)}%";
            _starView.SetStars(score.Stars);
        }

        internal void ResetScore()
        {
            _profileName.text = "";
            _scoreText.text = "";
            _percentText.text = $"-%";
            _starView.SetStars(0);
        }
    }
}