using System;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using YARG.Core;
using YARG.Player;
using YARG.Scores;

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
        [SerializeField]
        private Image _overlayGradient;
        [SerializeField]
        private Image _difficultyIcon;
        [SerializeField]
        private Image _difficultyIconBackground;
        [SerializeField]
        private CanvasGroup _canvasGroup;

        internal void Initialize(Difficulty difficulty)
        {
            ResetScore();
            var difficultyValue = difficulty switch
            {
                Difficulty.Easy => "E",
                Difficulty.Medium => "M",
                Difficulty.Hard => "H",
                Difficulty.Expert => "X",
                Difficulty.ExpertPlus => "XP",
                _ => ""
            };
            var icon = Addressables.LoadAssetAsync<Sprite>($"DifficultyIcons[Diff{difficultyValue}]").WaitForCompletion();
            _difficultyIcon.sprite = icon;
            _difficultyIconBackground.sprite = icon;
        }

        internal void SetScore(PlayerScoreRecord score)
        {
            var playerName = PlayerContainer.GetProfileById(score.PlayerId).Name;
            _profileName.text = playerName;
            _scoreText.text = score.Score.ToString();
            _percentText.text = $"{Math.Floor((float) score.Percent * 100)}%";
            _starView.SetStars(score.Stars);

            if (score.IsFc)
            {
                _overlayGradient.gameObject.SetActive(true);
            }
            else
            {
                _overlayGradient.gameObject.SetActive(false);
            }

            _canvasGroup.alpha = 1;
        }

        internal void ResetScore()
        {
            _profileName.text = "";
            _scoreText.text = "";
            _percentText.text = "";
            _starView.SetStars(0);
            _overlayGradient.gameObject.SetActive(false);
            _canvasGroup.alpha = 1;
        }

        internal void Disable()
        {
            _canvasGroup.alpha = 0;
        }
    }
}