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

        [Space]
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

        [Space]
        [SerializeField]
        private MoreInfoMenuScoreDisplay[] _scoreDisplays;

        [Space]
        [SerializeField]
        private Sprite[] _allContentRatingIcons;


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
            _backButton.SetDefaultButtonState(HelpBarButton.ButtonState.HOVER);

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

            _contentRatingIcon.sprite = _currentSong.SongRating switch
            {
                SongRating.Unspecified             => _allContentRatingIcons[0],
                SongRating.Family_Friendly         => _allContentRatingIcons[1],
                SongRating.Supervision_Recommended => _allContentRatingIcons[2],
                SongRating.Mature                  => _allContentRatingIcons[3],
                SongRating.No_Rating               => _allContentRatingIcons[0],
                SongRating.Sensitive_Content       => _allContentRatingIcons[4],
                _                                  => _allContentRatingIcons[0],
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

            UpdateIntensityIcons();

            _fcCountText.text = $"{ScoreContainer.GetFcCountForSong(_currentSong.Hash)}";
            _playCountText.text = $"{ScoreContainer.GetPlayCountForSong(_currentSong.Hash)}";

            _cancellationToken = new();
            LoadAlbumCover(_currentSong, _cancellationToken.Token).Forget();

            UpdateHighScores();
        }

        private void UpdateIntensityIcons()
        {
            /*
                Guitar               ; Bass               ; 4 lane      ; Keys     ; Vocals  ; Rhythm
                Pro Guitar           ; Pro Bass           ; 5 lane      ; Pro Keys ; Harmony ; Co-op
                                     ;                    ; Elite drums ;          ;         ; 6F
            */

            // Row 1
            _difficultyRings[0].SetInfo("guitar", Instrument.FiveFretGuitar, _currentSong[Instrument.FiveFretGuitar]);
            _difficultyRings[1].SetInfo("bass", Instrument.FiveFretBass, _currentSong[Instrument.FiveFretBass]);

            if (_currentSong.HasInstrument(Instrument.ProDrums))
            {
                _difficultyRings[2].SetInfo("realDrums", Instrument.ProDrums, _currentSong[Instrument.ProDrums]);
            }
            else
            {
                _difficultyRings[2].SetInfo("drums", Instrument.FourLaneDrums, _currentSong[Instrument.FourLaneDrums]);
            }

            _difficultyRings[3].SetInfo("keys", Instrument.Keys, _currentSong[Instrument.Keys]);

            _difficultyRings[4].SetInfo("vocals", Instrument.Vocals, _currentSong[Instrument.Vocals]);
            _difficultyRings[5].SetInfo("rhythm", Instrument.FiveFretRhythm, _currentSong[Instrument.FiveFretRhythm]);

            // Row 2
            var values = _currentSong[Instrument.ProGuitar_17Fret];
            var instrument = Instrument.ProGuitar_17Fret;
            if (values.Intensity == -1 && _currentSong.HasInstrument(Instrument.ProGuitar_22Fret))
            {
                values = _currentSong[Instrument.ProGuitar_22Fret];
                instrument = Instrument.ProGuitar_22Fret;
            }
            _difficultyRings[6].SetInfo("realGuitar", instrument, values);

            values = _currentSong[Instrument.ProBass_17Fret];
            instrument = Instrument.ProBass_17Fret;
            if (values.Intensity == -1 && _currentSong.HasInstrument(Instrument.ProBass_22Fret))
            {
                values = _currentSong[Instrument.ProBass_22Fret];
                instrument = Instrument.ProBass_22Fret;
            }
            _difficultyRings[7].SetInfo("realBass", instrument, values);

            _difficultyRings[8].SetInfo("ghDrums", Instrument.FiveLaneDrums, _currentSong[Instrument.FiveLaneDrums]);
            _difficultyRings[9].SetInfo("realKeys", Instrument.ProKeys, _currentSong[Instrument.ProKeys]);

            var partIcon = _currentSong.VocalsCount switch
            {
                >= 3 => "harmVocals",
                2    => "twoVocals",
                _    => "vocals",
            };
            _difficultyRings[10].SetInfo(partIcon, Instrument.Vocals, _currentSong[Instrument.Vocals]);
            _difficultyRings[11].SetInfo("guitarCoop", Instrument.FiveFretCoopGuitar, _currentSong[Instrument.FiveFretCoopGuitar]);

            // Row 3
            _difficultyRings[12].gameObject.SetActive(false);
            _difficultyRings[13].gameObject.SetActive(false);
            _difficultyRings[14].SetInfo("eliteDrums", Instrument.EliteDrums, _currentSong[Instrument.EliteDrums]);
            _difficultyRings[15].gameObject.SetActive(false);
            _difficultyRings[16].gameObject.SetActive(false);
            _difficultyRings[17].SetInfo("guitar6f", Instrument.SixFretGuitar, _currentSong[Instrument.SixFretGuitar]);
        }

        private void UpdateHighScores()
        {
            for (int i = 0; i < _scoreDisplays.Length; i++)
            {
                _scoreDisplays[i].ClearValues();
            }

            var highScores = ScoreContainer.GetHighScoresForSong(_currentSong.Hash);

            for (int i = 0; i < highScores.Count; i++)
            {
                var score = highScores[i];
                if (score.Instrument == Instrument.FiveFretBass)
                {
                    int difficulty = (int) score.Difficulty;
                    _scoreDisplays[difficulty].ShowScore(score);
                }
            }
        }

        // Album loading code stolen from Sidebar.cs
        private async UniTaskVoid LoadAlbumCover(SongEntry songEntry, CancellationToken cancellationToken)
        {
            Texture2D texture = null;

            // We explicitly don't use the cancellation token here as we need control to resume
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
