using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Text;
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
using YARG.Settings;
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

        [Space]
        [SerializeField]
        private GameObject _fcHighlight;
        [SerializeField]
        private GameObject _brutalFcHighlight;

        public void ClearValues()
        {
            _playerName.text = String.Empty;
            _scoreText.text = String.Empty;
            _percentText.text = String.Empty;
            _starView.gameObject.SetActive(false);
            _fcHighlight.SetActive(false);
            _brutalFcHighlight.SetActive(false);
        }

        public void ShowScore(PlayerScoreRecord scoreRecord)
        {
            using var scoreStringBuilder = ZString.CreateStringBuilder();
            var scoreColor = scoreRecord.IsFc ? "#ffd029" : "#ffffff";
            scoreStringBuilder.AppendFormat("<mspace=.5em><color={1}>{0:N0}</color></mspace>",
                scoreRecord.Score, scoreColor);

            if (SettingsManager.Settings.ShowPercentDecimals.Value)
            {
                var percent = Mathf.Floor(scoreRecord.GetPercent() * 1000f) / 10f;
                _percentText.text = $"<mspace=.5em><color={scoreColor}>{percent:0.0}</color></mspace><mspace=1em>%";
            }
            else
            {
                _percentText.text = $"<mspace=.5em><color={scoreColor}>{Mathf.FloorToInt(scoreRecord.GetPercent() * 100f)}<mspace=1em>%</color></mspace>";
            }

            _playerName.text = $"{scoreRecord.PlayerId}";
            _scoreText.text = scoreStringBuilder.ToString();
            _starView.gameObject.SetActive(true);
            _starView.SetStars(scoreRecord.Stars);
            _fcHighlight.SetActive(scoreRecord.IsFc);
        }
    }
}
