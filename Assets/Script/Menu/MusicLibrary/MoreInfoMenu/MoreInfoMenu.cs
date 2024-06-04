using System;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YARG.Core;
using YARG.Core.Input;
using YARG.Core.Logging;
using YARG.Core.Song;
using YARG.Helpers.Extensions;
using YARG.Menu.Navigation;
using YARG.Menu.Persistent;
using YARG.Scores;
using YARG.Song;

namespace YARG.Menu.MusicLibrary
{
    public class MoreInfoMenu : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _songTitle;
        [SerializeField]
        private TextMeshProUGUI _artistName;
        [SerializeField]
        private TextMeshProUGUI _albumTitle;
        [SerializeField]
        private TextMeshProUGUI _genreText;
        [SerializeField]
        private TextMeshProUGUI _songLengthText;
        [SerializeField]
        private TextMeshProUGUI _sourceName;
        [SerializeField]
        private TextMeshProUGUI _charterName;
        [SerializeField]
        private TextMeshProUGUI _loadingPhrase;

        [Space]
        [SerializeField]
        private RawImage _albumArt;
        [SerializeField]
        private RawImage _albumArtBackground;
        [SerializeField]
        private Image _sourceIcon;
        [SerializeField]
        private Image _sourceIconFooter;
        [SerializeField]
        private Image _sourceIconFooterBackground;
        
        // [Space]
        // [SerializeField]
        // private GameObject _menuItemPrefab;
        // [SerializeField]
        // private Transform _menuItemContainer;
        [Space]
        [SerializeField]
        private NavigationGroup _instrumentSelectNavGroup;
        [SerializeField]
        private HelpBarButton _redButton;
        [SerializeField]
        private HelpBarButton _blueButton;

        [Space]
        [SerializeField]
        private GameObject _difficultyRingPrefab;
        [SerializeField]
        private Transform _difficultyRingsTopContainer;
        [SerializeField]
        private Transform _difficultyRingsMiddleContainer;
        [SerializeField]
        private Transform _difficultyRingsBottomContainer;

        [Space]
        [SerializeField]
        private GameObject _scoreViewPrefab;
        [SerializeField]
        private Transform _scoreViewContainer;

        [Space]
        [SerializeField]
        private GameObject _buttonPrefab;
        [SerializeField]
        private Transform _instrumentDifficultyButtonsContainer;
        [SerializeField]
        private Transform _menuButtonsContainer;

        [Space]        
        [SerializeField]
        private NavigatableButton _playButton;
        [SerializeField]
        private NavigatableButton _practiceButton;

        [Space]
        [SerializeField]
        private Image _bandDifficultyBar;
        [SerializeField]
        private TextMeshProUGUI _bandDifficultyLabel;

        private readonly List<DifficultyRing> _difficultyRings = new();

        private readonly List<MoreInfoScoreView> _scoreViews = new();
        private Instrument[] _activeInstruments = new Instrument[12];
        private int _idx = 0;

        private List<Difficulty> _displayedDifficulties = new List<Difficulty>
        {
            Difficulty.Easy, Difficulty.Medium, Difficulty.Hard, Difficulty.Expert, Difficulty.ExpertPlus
        };

        private CancellationTokenSource _cancellationToken;
        private CancellationTokenSource _cancellationToken2;
        private SongEntry _currentSong;
        private List<PlayerScoreRecord> _scores;

        public void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            // _navGroup.ClearNavigatables();
            // _menuItemContainer.DestroyChildren();

            // var go = Instantiate(_menuItemPrefab, _menuItemContainer);
            // var btn = go.GetComponent<MoreInfoMenuItem>();
            // btn.Initialize("Play Song", true, (bool x) => {YargLogger.LogInfo("A");});

            // var go2 = Instantiate(_menuItemPrefab, _menuItemContainer);
            // var btn2 = go2.GetComponent<MoreInfoMenuItem>();
            // btn2.Initialize("Practice Song", false, (bool x) => {YargLogger.LogInfo("B");});

            // var go3 = Instantiate(_menuItemPrefab, _menuItemContainer);
            // var btn3 = go3.GetComponent<MoreInfoMenuItem>();
            // btn3.Initialize("Rating", false, (bool x) => {YargLogger.LogInfo("C");});

            // _navGroup.AddNavigatable(btn);
            // _navGroup.AddNavigatable(btn2);
            // _navGroup.AddNavigatable(btn3);

            // btn.SetSelected(true, SelectionOrigin.Mouse);

            // var instrumentHeaders = new List<string> {"Guitar", "Bass", "Drums", "Vocals", "Keys"};
            // foreach (var instrumentHeader in instrumentHeaders)
            // {
            //     var go4 = Instantiate(_instrumentHeaderPrefab, _instrumentHeaderContainer);
            //     var header = go4.GetComponent<InstrumentHeader>();
            //     header.SetText(instrumentHeader);
            //     header.SetSelected(false);
            //     _instrumentHeaderList.Add(header);
            // }
            // _instrumentHeaderList[_instrumentIdx].SetSelected(true);

            for (int i = 0; i < 5; ++i)
            {
                var go = Instantiate(_difficultyRingPrefab, _difficultyRingsTopContainer);
                _difficultyRings.Add(go.GetComponent<DifficultyRing>());
            }

            for (int i = 0; i < 5; ++i)
            {
                var go = Instantiate(_difficultyRingPrefab, _difficultyRingsMiddleContainer);
                _difficultyRings.Add(go.GetComponent<DifficultyRing>());
            }

            // for (int i = 0; i < 5; ++i)
            // {
            //     var go = Instantiate(_difficultyRingPrefab, _difficultyRingsBottomContainer);
            //     _difficultyRings.Add(go.GetComponent<DifficultyRing>());
            // }

            for (int i = 0; i < _displayedDifficulties.Count; ++i)
            {
                var go = Instantiate(_scoreViewPrefab, _scoreViewContainer);
                var scoreView = go.GetComponent<MoreInfoScoreView>();
                _scoreViews.Add(scoreView);
                scoreView.Initialize(_displayedDifficulties[i]);
            }

            _instrumentSelectNavGroup.SelectionChanged += UpdateDisplayedScores;
        }

        private void UpdateDifficulties()
        {
            var entry = _currentSong;
            // Show all difficulty rings
            foreach (var difficultyRing in _difficultyRings)
            {
                difficultyRing.gameObject.SetActive(true);
            }

            /*
                Guitar               ; Bass               ; 4 lane      ; Keys     ; Mic
                Pro Guitar           ; Pro Bass           ; True Drums  ; Pro Keys ; Harmonies
                Co-op                ; Rhythm             ; GH Drums
            */

            _difficultyRings[0].SetInfo("guitar", "FiveFretGuitar", entry[Instrument.FiveFretGuitar]);
            _difficultyRings[1].SetInfo("bass", "FiveFretBass", entry[Instrument.FiveFretBass]);
            _difficultyRings[2].SetInfo("drums", "FourLaneDrums", entry[Instrument.FourLaneDrums]);

            // 5-lane or 4-lane
            // if (entry.HasInstrument(Instrument.FiveLaneDrums))
            // {
            //     _difficultyRings[2].SetInfo("ghDrums", "FiveLaneDrums", entry[Instrument.FiveLaneDrums]);
            // }

            _difficultyRings[3].SetInfo("keys", "Keys", entry[Instrument.Keys]);
            _difficultyRings[4].SetInfo("vocals", "Vocals", entry[Instrument.Vocals]);

            // if (entry.HasInstrument(Instrument.Harmony))
            // {
            //     _difficultyRings[4].SetInfo(
            //         entry.VocalsCount switch
            //         {
            //             2 => "twoVocals",
            //             >= 3 => "harmVocals",
            //             _ => "vocals"
            //         },
            //         "Harmony",
            //         entry[Instrument.Harmony]
            //     );
            // }
            // else
            // {
                
            // }

            _difficultyRings[5].SetInfo("realGuitar", "ProGuitar", entry[Instrument.ProGuitar_17Fret]);

            // Protar or Co-op
            // if (entry.HasInstrument(Instrument.ProGuitar_17Fret) || entry.HasInstrument(Instrument.ProGuitar_22Fret))
            // {
            //     var values = entry[Instrument.ProGuitar_17Fret];
            //     if (values.Intensity == -1)
            //         values = entry[Instrument.ProGuitar_22Fret];
            //     _difficultyRings[5].SetInfo("realGuitar", "ProGuitar", values);
            // }
            // else
            // {
            //     _difficultyRings[5].SetInfo("guitarCoop", "FiveFretCoopGuitar", entry[Instrument.FiveFretCoopGuitar]);
            // }

            _difficultyRings[6].SetInfo("realBass", "ProBass", entry[Instrument.ProBass_17Fret]);

            // ProBass or Rhythm
            // if (entry.HasInstrument(Instrument.ProBass_17Fret) || entry.HasInstrument(Instrument.ProBass_22Fret))
            // {
            //     var values = entry[Instrument.ProBass_17Fret];
            //     if (values.Intensity == -1)
            //         values = entry[Instrument.ProBass_22Fret];
            //     _difficultyRings[6].SetInfo("realBass", "ProBass", values);
            // }
            // else
            // {
            //     _difficultyRings[6].SetInfo("rhythm", "FiveFretRhythm", entry[Instrument.FiveFretRhythm]);
            // }

            _difficultyRings[7].SetInfo("trueDrums", "TrueDrums", PartValues.Default);
            _difficultyRings[8].SetInfo("realKeys", "ProKeys", entry[Instrument.ProKeys]);
            _difficultyRings[9].SetInfo(
                entry.VocalsCount switch
                {
                    2 => "twoVocals",
                    >= 3 => "harmVocals",
                    _ => "vocals"
                },
                "Harmony",
                entry[Instrument.Harmony]
            );

            _bandDifficultyLabel.text = entry[Instrument.Band].Intensity == -1 ? "-" : entry[Instrument.Band].Intensity.ToString();
            _bandDifficultyBar.fillAmount = Math.Clamp((float) entry[Instrument.Band].Intensity / 5, 0, 1);
        }
        
        private void OnEnable()
        {
            var redEntry = new NavigationScheme.Entry(MenuAction.Red, "Back", () =>
            {
                gameObject.SetActive(false);
            });
            var blueEntry = new NavigationScheme.Entry(MenuAction.Blue, "Change Instrument", () =>
            {
                if (_instrumentSelectNavGroup.SelectedIndex == _instrumentSelectNavGroup.Count - 1)
                {
                    _instrumentSelectNavGroup.SelectFirst();
                }
                else
                {
                    _instrumentSelectNavGroup.SelectNext();
                }
            });

            Navigator.Instance.PushScheme(new NavigationScheme(new()
            {
                NavigationScheme.Entry.NavigateUp,
                NavigationScheme.Entry.NavigateDown,
                NavigationScheme.Entry.NavigateSelect,
                redEntry,
                blueEntry
            }, false));

            _redButton.SetInfoFromSchemeEntry(redEntry);
            _blueButton.SetInfoFromSchemeEntry(blueEntry);

            _currentSong = GlobalVariables.State.CurrentSong;

            _songTitle.text = _currentSong.Name;
            _artistName.text = _currentSong.Artist;

            _albumTitle.text = $"{_currentSong.Album}, {_currentSong.Year}";
            _genreText.text = _currentSong.Genre;
            var time = TimeSpan.FromMilliseconds(_currentSong.SongLengthMilliseconds);
            if (time.Hours > 0)
            {
                _songLengthText.text = time.ToString(@"h\:mm\:ss");
            }
            else
            {
                _songLengthText.text = time.ToString(@"m\:ss");
            }

            if (SongSources.TryGetSource(_currentSong.Source, out var parsedSource))
            {
                _sourceName.text = parsedSource.GetDisplayName();
            }
            else
            {
                _sourceName.text = SongSources.Default.GetDisplayName();
            }
            _charterName.text = _currentSong.Charter;
            if (_currentSong.LoadingPhrase == null)
            {
                _loadingPhrase.gameObject.SetActive(false);
            }
            else
            {
                _loadingPhrase.text = _currentSong.LoadingPhrase;
            }

            UpdateDifficulties();
            UpdateInstrumentSelectionButtons();
            FetchScores();
            _instrumentSelectNavGroup.SelectFirst();
            LoadSourceIcons();
            LoadAlbumCover();
        }

        private void FetchScores()
        {
            _scores = ScoreContainer.GetHighScoresByInstrumentAndDifficulty(_currentSong.Hash);
            YargLogger.LogInfo($"Fetched {_scores.Count} scores");
        }

        private void CreateInstrumentButton(string buttonText, Instrument instrument)
        {
            if (_currentSong[instrument].WasParsed())
            {
                var go = Instantiate(_buttonPrefab, _instrumentDifficultyButtonsContainer);
                var button = go.GetComponent<MoreInfoMenuButton>();
                button.Text.text = buttonText;
                button.Text.fontSizeMax = 22;
                button.Icon.gameObject.SetActive(false);
                _instrumentSelectNavGroup.AddNavigatable(button.Button);
                _activeInstruments[_idx] = instrument;
                _idx++;
            }
        }

        private void UpdateInstrumentSelectionButtons()
        {
            _idx = 0;
            _instrumentDifficultyButtonsContainer.DestroyChildren();
            _instrumentSelectNavGroup.ClearNavigatables();

            CreateInstrumentButton("Guitar", Instrument.FiveFretGuitar);
            CreateInstrumentButton("Bass", Instrument.FiveFretBass);
            CreateInstrumentButton("Drums", Instrument.FourLaneDrums);
            CreateInstrumentButton("Vocals", Instrument.Vocals);
            CreateInstrumentButton("Keys", Instrument.Keys);
            CreateInstrumentButton("Pro Guitar", Instrument.ProGuitar_17Fret);
            CreateInstrumentButton("Pro Bass", Instrument.ProBass_17Fret);
            CreateInstrumentButton("Pro Drums", Instrument.ProDrums);
            CreateInstrumentButton("Pro Keys", Instrument.ProKeys);
            CreateInstrumentButton("Rhythm", Instrument.FiveFretRhythm);
            CreateInstrumentButton("Co-op", Instrument.FiveFretCoopGuitar);
            CreateInstrumentButton("5 Lane Drums", Instrument.FiveLaneDrums);
        }

        private void UpdateDisplayedScores(NavigatableBehaviour selected, SelectionOrigin selectionOrigin)
        {
            var selectedIndex = _instrumentSelectNavGroup.SelectedIndex;

            if (selectedIndex == null)
                return;
            
            var instrument = _activeInstruments[(int) selectedIndex];
            for (int i = 0; i < _displayedDifficulties.Count; ++i)
            {
                if (HasPlayableDifficulty(_currentSong, instrument, _displayedDifficulties[i]))
                {
                    _scoreViews[i].ResetScore();
                }
                else
                {
                    _scoreViews[i].Disable();
                }
            }

            for (int i = 0; i < _scores.Count; ++i)
            {
                var _score = _scores[i];
                if (_score.Instrument == instrument)
                {
                    var scoreView = _scoreViews[(int) _score.Difficulty - 1];
                    scoreView.SetScore(_score);
                }
            }
        }

        // copied from DifficultySelectMenu
        private bool HasPlayableDifficulty(SongEntry entry, in Instrument instrument, in Difficulty difficulty)
        {
            // For vocals, insert special difficulties
            if (instrument is Instrument.Vocals or Instrument.Harmony)
            {
                return difficulty is not (Difficulty.Beginner or Difficulty.ExpertPlus);
            }

            // Otherwise, we can do this
            return entry[instrument][difficulty] || instrument switch
            {
                // Allow 5 -> 4-lane conversions to be played on 4-lane
                Instrument.FourLaneDrums or
                Instrument.ProDrums      => entry[Instrument.FiveLaneDrums][difficulty],
                // Allow 4 -> 5-lane conversions to be played on 5-lane
                Instrument.FiveLaneDrums => entry[Instrument.ProDrums][difficulty],
                _ => false
            };
        }

        private void OnDisable()
        {
            Navigator.Instance.PopScheme();
        }

        public async void LoadSourceIcons()
        {
            if (_currentSong == null) return;

            _sourceIcon.gameObject.SetActive(false);
            _sourceIconFooter.gameObject.SetActive(false);
            _sourceIconFooterBackground.gameObject.SetActive(false);

            try
            {
                var token = new CancellationToken();
                var icon = await SongSources.SourceToIcon(_currentSong.Source);

                token.ThrowIfCancellationRequested();

                if (icon != null)
                {
                    _sourceIcon.gameObject.SetActive(true);
                    _sourceIcon.sprite = icon;
                    _sourceIconFooter.gameObject.SetActive(true);
                    _sourceIconFooter.sprite = icon;
                    _sourceIconFooterBackground.gameObject.SetActive(true);
                    _sourceIconFooterBackground.sprite = icon;
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        public async void LoadAlbumCover()
        {
            if (_currentSong == null) return;

            var originalTexture = _albumArt.texture;

            // Load the new one
            _cancellationToken = new();
            _cancellationToken2 = new();
            await _albumArt.LoadAlbumCover(GlobalVariables.State.CurrentSong, _cancellationToken.Token);
            await _albumArtBackground.LoadAlbumCover(GlobalVariables.State.CurrentSong, _cancellationToken2.Token);

            // Dispose of the old texture (prevent memory leaks)
            if (originalTexture != null)
            {
                Destroy(originalTexture);
            }
        }

        private void Back()
        {
            MenuManager.Instance.PopMenu();
        }
    }
}