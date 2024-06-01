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
        //[SerializeField]
        //private NavigationGroup _navGroup;
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
        private Transform _difficultyRingsBottomContainer;

        [Space]
        [SerializeField]
        private GameObject _scoreViewPrefab;
        [SerializeField]
        private Transform _scoreViewContainer;

        [Space]        
        [SerializeField]
        private NavigatableButton _playButton;
        [SerializeField]
        private NavigatableButton _practiceButton;

        private readonly List<DifficultyRing> _difficultyRings = new();

        private readonly List<MoreInfoScoreView> _scoreViews = new();

        // [Space]
        // [SerializeField]
        // private GameObject _instrumentHeaderPrefab;
        // [SerializeField]
        // private Transform _instrumentHeaderContainer;
        // private List<InstrumentHeader> _instrumentHeaderList = new ();
        private int _instrumentIdx = 0;

        private CancellationTokenSource _cancellationToken;
        private CancellationTokenSource _cancellationToken2;
        private SongEntry _currentSong;

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
                var go = Instantiate(_difficultyRingPrefab, _difficultyRingsBottomContainer);
                _difficultyRings.Add(go.GetComponent<DifficultyRing>());
            }

            for (int i = 0; i < 5; ++i)
            {
                var go = Instantiate(_scoreViewPrefab, _scoreViewContainer);
                _scoreViews.Add(go.GetComponent<MoreInfoScoreView>());
            }
        }
        
        private void OnEnable()
        {
            //_navGroup.ClearNavigatables();

            // _navGroup.AddNavigatable(_playButton);
            // _navGroup.AddNavigatable(_practiceButton);

            var redEntry = new NavigationScheme.Entry(MenuAction.Red, "Back", () =>
            {
                gameObject.SetActive(false);
            });
            var blueEntry = new NavigationScheme.Entry(MenuAction.Blue, "Change Instrument", () => {});

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

            // _infoContainers.SelectInstrument(_instrumentIdx);
            LoadSourceIcons();
            LoadAlbumCover();
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

        // public void CycleInstrument()
        // {
        //     _instrumentHeaderList[_instrumentIdx].SetSelected(false);
        //     _instrumentIdx = (_instrumentIdx + 1) % 5;
        //     _infoContainers.SelectInstrument(_instrumentIdx);
        //     _instrumentHeaderList[_instrumentIdx].SetSelected(true);
        // }
    }
}