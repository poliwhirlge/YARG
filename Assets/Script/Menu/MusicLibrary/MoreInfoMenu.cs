using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using YARG.Core;
using YARG.Core.Input;
using YARG.Core.Song;
using YARG.Core.Utility;
using YARG.Helpers.Extensions;
using YARG.Localization;
using YARG.Menu.Navigation;
using YARG.Menu.Persistent;
using YARG.Scores;
using YARG.Settings;
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
        private HelpBarButton _toggleSoloBandScoresButton;
        [SerializeField]
        private HelpBarButton _toggleInstrumentScoresButton;
        [SerializeField]
        private MoreInfoMenuScoreDisplay[] _scoreDisplays;

        [Space]
        [SerializeField]
        private Sprite[] _allContentRatingIcons;


        private          SongEntry               _currentSong;
        private          CancellationTokenSource _cancellationToken;
        private readonly Color                   _bandDifficultyGray      = new Color(20 / 255f, 20 / 255f, 20 / 255f, 1f);
        private readonly Color                   _bandDifficultyRed       = new Color(251 / 255f, 68 / 255f, 63 / 255f, 1);
        private readonly Color                   _bandDifficultyBlue      = new Color(46 / 255f, 217 / 255f, 255 / 255f, 1);
        private readonly Difficulty[]            _difficulties            = { Difficulty.Beginner, Difficulty.Easy, Difficulty.Medium, Difficulty.Hard, Difficulty.Expert, Difficulty.ExpertPlus };
        private          Instrument[]            _allInstruments          = (Instrument[])Enum.GetValues(typeof(Instrument));
        private          List<Instrument>        _availableInstruments    = new();
        private          int                     _selectedInstrumentIndex = 0;
        private          Instrument              _selectedInstrument      = Instrument.FiveFretGuitar;

        private List<Instrument> _unplayableInstruments = new List<Instrument>()
        {
            Instrument.Band
        };

        private void OnEnable()
        {
            var redEntry = new NavigationScheme.Entry(MenuAction.Red, "Back", () => gameObject.SetActive(false));
            var yellowEntry = new NavigationScheme.Entry(MenuAction.Yellow, "Back", () => gameObject.SetActive(false));
            var blueEntry = new NavigationScheme.Entry(MenuAction.Blue, "Blue", CycleInstrument);
            Navigator.Instance.PushScheme(new NavigationScheme(new()
            {
                // NavigationScheme.Entry.NavigateUp,
                // NavigationScheme.Entry.NavigateDown,
                // NavigationScheme.Entry.NavigateSelect,
                redEntry,
                yellowEntry,
                blueEntry
            }, false));
            _backButton.SetInfoFromSchemeEntry(redEntry);
            _backButton.SetDefaultButtonState(HelpBarButton.ButtonState.HOVER);
            _toggleSoloBandScoresButton.SetInfoFromSchemeEntry(yellowEntry);
            _toggleInstrumentScoresButton.SetInfoFromSchemeEntry(blueEntry);

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

            var songRating = _currentSong.GetSongRating(SettingsManager.Settings.CensorMatureContent.Value);
            _contentRatingText.text = songRating switch
            {
                SongRating.Unspecified             => "No Rating",
                SongRating.Family_Friendly         => "Family Friendly",
                SongRating.Supervision_Recommended => "Supervision Recommended",
                SongRating.Mature                  => "Mature Content",
                SongRating.No_Rating               => "No Rating",
                SongRating.Sensitive_Content       => "Sensitive Content",
                _                                  => "No Rating",
            };

            _contentRatingIcon.sprite = songRating switch
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

            UpdateAvailableInstruments();
            UpdateInstrumentSelection();
            UpdateHighScores();
        }

        private void UpdateIntensityIcons()
        {
            /*
                Guitar               ; Bass               ; 4 lane      ; Keys     ; Vocals  ; Rhythm     ; Co-op(Melody)
                Pro Guitar           ; Pro Bass           ; Elite Drums ; Pro Keys ; Harmony ; Pro Rhythm ; Pro Co-op
                6F Guitar            ; 6F Bass            ; 5 lane      ; 6L Keys  ;         ; 6F Rhythm  ; 6F Co-op
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
            _difficultyRings[6].SetInfo("guitarCoop", Instrument.FiveFretCoopGuitar, _currentSong[Instrument.FiveFretCoopGuitar]);

            // Row 2
            var values = _currentSong[Instrument.ProGuitar_17Fret];
            var instrument = Instrument.ProGuitar_17Fret;
            if (values.Intensity == -1 && _currentSong.HasInstrument(Instrument.ProGuitar_22Fret))
            {
                values = _currentSong[Instrument.ProGuitar_22Fret];
                instrument = Instrument.ProGuitar_22Fret;
            }
            _difficultyRings[7].SetInfo("realGuitar", instrument, values);

            values = _currentSong[Instrument.ProBass_17Fret];
            instrument = Instrument.ProBass_17Fret;
            if (values.Intensity == -1 && _currentSong.HasInstrument(Instrument.ProBass_22Fret))
            {
                values = _currentSong[Instrument.ProBass_22Fret];
                instrument = Instrument.ProBass_22Fret;
            }
            _difficultyRings[8].SetInfo("realBass", instrument, values);
            _difficultyRings[9].SetInfo("eliteDrums", Instrument.EliteDrums, _currentSong[Instrument.EliteDrums]);
            _difficultyRings[10].SetInfo("realKeys", Instrument.ProKeys, _currentSong[Instrument.ProKeys]);

            var partIcon = _currentSong.VocalsCount switch
            {
                >= 3 => "harmVocals",
                _    => "twoVocals",
            };
            _difficultyRings[11].SetInfo(partIcon, Instrument.Harmony, _currentSong[Instrument.Harmony]);
            _difficultyRings[12].gameObject.SetActive(false);
            _difficultyRings[13].gameObject.SetActive(false);

            // Row 3
            _difficultyRings[14].SetInfo("guitar6f", Instrument.SixFretGuitar, _currentSong[Instrument.SixFretGuitar]);
            _difficultyRings[15].SetInfo("bass6f", Instrument.SixFretBass, _currentSong[Instrument.SixFretBass]);
            _difficultyRings[16].SetInfo("ghDrums", Instrument.FiveLaneDrums, _currentSong[Instrument.FiveLaneDrums]);
            _difficultyRings[17].gameObject.SetActive(false);
            _difficultyRings[18].gameObject.SetActive(false);
            _difficultyRings[19].SetInfo("rhythm6f", Instrument.SixFretRhythm, _currentSong[Instrument.SixFretRhythm]);
            _difficultyRings[20].SetInfo("guitarCoop6f", Instrument.SixFretCoopGuitar, _currentSong[Instrument.SixFretCoopGuitar]);
        }

        private void UpdateAvailableInstruments()
        {
            _availableInstruments.Clear();

            for (int i = 0; i < _allInstruments.Length; i++)
            {
                Instrument instrument = _allInstruments[i];

                if (_currentSong[instrument].IsActive() && !_unplayableInstruments.Contains(instrument))
                {
                    _availableInstruments.Add(instrument);
                }
            }
        }

        private void UpdateInstrumentSelection()
        {
            _selectedInstrumentIndex = 0;

            for (int i = 0; i < _availableInstruments.Count; i++)
            {
                Instrument instrument = _availableInstruments[i];

                if (_selectedInstrument == instrument)
                {
                    _selectedInstrumentIndex = i;
                }
            }

            _selectedInstrument = _availableInstruments[_selectedInstrumentIndex];

            _toggleInstrumentScoresButton.SetButtonLabel(Localize.ToLocalizedName(_selectedInstrument));
        }

        private void CycleInstrument()
        {
            _selectedInstrumentIndex += 1;

            if (_selectedInstrumentIndex >= _availableInstruments.Count)
            {
                _selectedInstrumentIndex = 0;
            }

            _selectedInstrument = _availableInstruments[_selectedInstrumentIndex];
            _toggleInstrumentScoresButton.SetButtonLabel(Localize.ToLocalizedName(_selectedInstrument));
            UpdateHighScores();
        }

        private void UpdateHighScores()
        {
            for (int i = 0; i < _scoreDisplays.Length; i++)
            {
                var difficulty = _difficulties[i];
                var hasDifficulty = false;

                if (_selectedInstrument == Instrument.Vocals || _selectedInstrument == Instrument.Harmony)
                {
                    hasDifficulty = difficulty != Difficulty.ExpertPlus;
                }
                else
                {
                    hasDifficulty = _currentSong.HasDifficultyForInstrument(_selectedInstrument, difficulty);
                }

                _scoreDisplays[i].ClearValues(hide: !hasDifficulty);
            }

            var highScores = ScoreContainer.GetHighScoresForSong(_currentSong.Hash);

            for (int i = 0; i < highScores.Count; i++)
            {
                var score = highScores[i];
                if (score.Instrument == _selectedInstrument)
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
