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
using YARG.Song;
using static System.Globalization.CultureInfo;

namespace YARG.Menu.MusicLibrary
{
    public class MoreInfoMenu : MonoBehaviour
    {
        [SerializeField]
        private HelpBarButton _backButton;
        [SerializeField]
        private GameObject _creditLicenseBar;
        [SerializeField]
        private TextMeshProUGUI _creditLicenseText;
        [SerializeField]
        private RawImage _albumCoverBackground;
        [SerializeField]
        private RawImage _albumCoverSmall;

        [Space]
        [SerializeField]
        private TextMeshProUGUI _songText;
        [SerializeField]
        private TextMeshProUGUI _artistText;
        [SerializeField]
        private TextMeshProUGUI _albumYearText;
        [SerializeField]
        private TextMeshProUGUI _genreText;
        [SerializeField]
        private TextMeshProUGUI _lengthText;
        [SerializeField]
        private TextMeshProUGUI _sourceText;
        [SerializeField]
        private TextMeshProUGUI _contentRatingText;
        [SerializeField]
        private TextMeshProUGUI _playCountText;
        [SerializeField]
        private TextMeshProUGUI _fcCountText;
        [SerializeField]
        private Image _contentRatingIcon;
        [SerializeField]
        private Image _sourceIcon;
        [SerializeField]
        private Image _bandBarImage;
        [SerializeField]
        private TextMeshProUGUI _bandBarText;
        [SerializeField]
        private DifficultyRing[] _difficultyRings;

        [Space]
        [SerializeField]
        private Image _charterIcon;
        [SerializeField]
        private Image _charterIconBackground;
        [SerializeField]
        private TextMeshProUGUI _charterText;
        [SerializeField]
        private GameObject _loadingPhraseBar;
        [SerializeField]
        private TextMeshProUGUI _loadingPhraseText;


        private          SongEntry               _currentSong;
        private          CancellationTokenSource _cancellationToken;
        private readonly Color                   _bandDifficultyGray = new Color(20 / 255f, 20 / 255f, 20 / 255f, 1f);
        private readonly Color                   _bandDifficultyRed  = new Color(251 / 255f, 68 / 255f, 63 / 255f, 1);
        private readonly Color                   _bandDifficultyBlue = new Color(46 / 255f, 217 / 255f, 255 / 255f, 1);

        private void OnEnable()
        {
            var redEntry = new NavigationScheme.Entry(MenuAction.Red, "Back", () => gameObject.SetActive(false));
            Navigator.Instance.PushScheme(new NavigationScheme(new()
            {
                // NavigationScheme.Entry.NavigateUp,
                // NavigationScheme.Entry.NavigateDown,
                // NavigationScheme.Entry.NavigateSelect,
                redEntry
            }, false));
            _backButton.SetInfoFromSchemeEntry(redEntry);

            _currentSong = GlobalVariables.State.CurrentSong;
            UpdateSongInfo();
        }

        private void UpdateSongInfo()
        {
            _songText.text = _currentSong.Name;
            _artistText.text = _currentSong.Artist;
            _genreText.text = _currentSong.Genre;
            _charterText.text = _currentSong.Charter;
            _sourceText.text = SongSources.SourceToGameName(_currentSong.Source);
            _contentRatingText.text = _currentSong.SongRating switch
            {
                SongRating.Unspecified             => "No Rating",
                SongRating.Family_Friendly         => "Family Friendly",
                SongRating.Supervision_Recommended => "Supervision Recommended",
                SongRating.Mature                  => "Mature Content",
                SongRating.No_Rating               => "No Rating",
                SongRating.Sensitive_Content       => "Sensitive Content",
                _                                  => "No Rating",
            };

            if (!string.IsNullOrEmpty(_currentSong.LoadingPhrase))
            {
                _loadingPhraseText.text = _currentSong.LoadingPhrase;
                _loadingPhraseBar.SetActive(true);
            }
            else
            {
                _loadingPhraseText.text = String.Empty;
                _loadingPhraseBar.SetActive(false);
            }

            if (!string.IsNullOrEmpty(_currentSong.CreditLicense))
            {
                _creditLicenseText.text = _currentSong.CreditLicense;
                _creditLicenseBar.SetActive(true);
            }
            else
            {
                _creditLicenseText.text = String.Empty;
                _creditLicenseBar.SetActive(false);
            }

            if (!string.IsNullOrEmpty(_currentSong.YearSecondary))
            {
                _albumYearText.text = $"{_currentSong.Album}, {_currentSong.ParsedYear} ({_currentSong.YearSecondary})";
            }
            else
            {
                _albumYearText.text = $"{_currentSong.Album}, {_currentSong.ParsedYear}";
            }

            var time = TimeSpan.FromMilliseconds(_currentSong.SongLengthMilliseconds);
            if (time.Hours > 0)
            {
                _lengthText.text = time.ToString(@"h\:mm\:ss");
            }
            else
            {
                _lengthText.text = time.ToString(@"m\:ss");
            }

            var icon = SongSources.SourceToIcon(_currentSong.Source);

            if (icon)
            {
                _sourceIcon.sprite = icon;
                _charterIcon.sprite = icon;
                _charterIconBackground.sprite = icon;
            }

            int bandIntensity = _currentSong[Instrument.Band].Intensity;
            _bandBarText.text = bandIntensity == -1 ? "-" : bandIntensity.ToString();
            _bandBarImage.fillAmount = Math.Clamp((float) bandIntensity / 5, 0, 1);

            if (bandIntensity >= 6)
            {
                _bandBarImage.color = _bandDifficultyRed;
            }
            else if (bandIntensity < 0)
            {
                _bandBarImage.color = _bandDifficultyGray;
            }
            else
            {
                _bandBarImage.color = _bandDifficultyBlue;
            }

            _cancellationToken = new();
            LoadAlbumCover(_currentSong, _cancellationToken.Token).Forget();
        }

        // Album loading code stolen from Sidebar.cs
        private async UniTaskVoid LoadAlbumCover(SongEntry songEntry, CancellationToken cancellationToken)
        {
            Texture2D texture = null;

            // We explicity don't use the cancellation token here as we need control to resume
            // in *this method* to ensure that image gets disposed since it is backed by a FixedArray
            // ReSharper disable once MethodSupportsCancellation
            using var image = await UniTask.RunOnThreadPool(songEntry.LoadAlbumData);
            if (image != null)
            {
                texture = image.LoadTexture(false);
            }

            if (cancellationToken.IsCancellationRequested)
            {
                if (texture != null)
                {
                    Destroy(texture);
                }
                return;
            }

            ClearAlbumCoverTextures();

            SetAlbumCover(_albumCoverBackground, texture, 1f);
            SetAlbumCover(_albumCoverSmall, texture, 1f);
        }

        private void ClearAlbumCoverTextures()
        {
            var mainTexture = _albumCoverBackground.texture;
            var smallTexture = _albumCoverSmall.texture;

            if (mainTexture != null)
            {
                Destroy(mainTexture);
            }

            if (smallTexture != null && !ReferenceEquals(mainTexture, smallTexture))
            {
                Destroy(smallTexture);
            }

            SetAlbumCover(_albumCoverBackground, null, 1f);
            SetAlbumCover(_albumCoverSmall, null, 1f);
        }

        private static void SetAlbumCover(RawImage image, Texture2D texture, float alpha)
        {
            image.texture = texture;
            image.uvRect = new Rect(0f, 0f, 1f, -1f);
            image.color = texture != null ? Color.white.WithAlpha(alpha) : Color.clear;
        }

        private void OnDisable()
        {
            Navigator.Instance.PopScheme();
        }


    }
}
