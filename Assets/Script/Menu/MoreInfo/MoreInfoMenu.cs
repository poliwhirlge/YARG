using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using YARG.Core.Input;
using YARG.Core.Logging;
using YARG.Helpers.Extensions;
using YARG.Menu;
using YARG.Menu.MusicLibrary;
using YARG.Menu.Navigation;
using YARG.Song;

namespace YARG.Menu.MoreInfo
{
    public class MoreInfoMenu : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _subHeader;

        [SerializeField]
        private TextMeshProUGUI _songTitle;
        [SerializeField]
        private TextMeshProUGUI _albumTitle;
        [SerializeField]
        private TextMeshProUGUI _artistName;
        [SerializeField]
        private TextMeshProUGUI _sourceName;
        [SerializeField]
        private InfoContainers _infoContainers;
        
        [Space]
        [SerializeField]
        private GameObject _menuItemPrefab;
        [SerializeField]
        private Transform _menuItemContainer;
        [SerializeField]
        private NavigationGroup _navGroup;

        [Space]
        [SerializeField]
        private GameObject _instrumentHeaderPrefab;
        [SerializeField]
        private Transform _instrumentHeaderContainer;
        private List<InstrumentHeader> _instrumentHeaderList = new ();
        private int _instrumentIdx = 0;

        private bool _initialized = false;

        public void Initialize()
        {
            _navGroup.ClearNavigatables();
            _menuItemContainer.DestroyChildren();

            var go = Instantiate(_menuItemPrefab, _menuItemContainer);
            var btn = go.GetComponent<MoreInfoMenuItem>();
            btn.Initialize("Play Song", true, (bool x) => {YargLogger.LogInfo("A");});

            var go2 = Instantiate(_menuItemPrefab, _menuItemContainer);
            var btn2 = go2.GetComponent<MoreInfoMenuItem>();
            btn2.Initialize("Practice Song", false, (bool x) => {YargLogger.LogInfo("B");});

            var go3 = Instantiate(_menuItemPrefab, _menuItemContainer);
            var btn3 = go3.GetComponent<MoreInfoMenuItem>();
            btn3.Initialize("Rating", false, (bool x) => {YargLogger.LogInfo("C");});

            _navGroup.AddNavigatable(btn);
            _navGroup.AddNavigatable(btn2);
            _navGroup.AddNavigatable(btn3);

            btn.SetSelected(true, SelectionOrigin.Mouse);

            var instrumentHeaders = new List<string> {"Guitar", "Bass", "Drums", "Vocals", "Keys"};
            foreach (var instrumentHeader in instrumentHeaders)
            {
                var go4 = Instantiate(_instrumentHeaderPrefab, _instrumentHeaderContainer);
                var header = go4.GetComponent<InstrumentHeader>();
                header.SetText(instrumentHeader);
                header.SetSelected(false);
                _instrumentHeaderList.Add(header);
            }
            _instrumentHeaderList[_instrumentIdx].SetSelected(true);

            _initialized = true;
        }

        public void Awake()
        {
        }
        
        private void OnEnable()
        {
            if (!_initialized)
            {
                Initialize();
            }

            Navigator.Instance.PushScheme(new NavigationScheme(new()
            {
                new NavigationScheme.Entry(MenuAction.Green, "Confirm", () => {_navGroup.SelectedBehaviour?.Confirm();}),
                new NavigationScheme.Entry(MenuAction.Red, "Back", Back),
                new NavigationScheme.Entry(MenuAction.Blue, "Next Instrument", () =>
                {
                    CycleInstrument();
                }),
            }, false));

            _subHeader.text = MusicLibraryMenu.LibraryMode switch
            {
                MusicLibraryMode.QuickPlay => "Quickplay",
                MusicLibraryMode.Practice => "Practice",
                _ => throw new Exception("Unreachable.")
            };

            _songTitle.text = GlobalVariables.State.CurrentSong.Name;
            _albumTitle.text = GlobalVariables.State.CurrentSong.Album;
            _artistName.text = GlobalVariables.State.CurrentSong.Artist;
            if (SongSources.TryGetSource(GlobalVariables.State.CurrentSong.Source, out var parsedSource))
            {
                _sourceName.text = parsedSource.GetDisplayName();
            }
            else
            {
                _sourceName.text = SongSources.Default.GetDisplayName();
            }

            _infoContainers.SelectInstrument(_instrumentIdx);
        }

        private void Back()
        {
            MenuManager.Instance.PopMenu();
        }

        public void CycleInstrument()
        {
            _instrumentHeaderList[_instrumentIdx].SetSelected(false);
            _instrumentIdx = (_instrumentIdx + 1) % 5;
            _infoContainers.SelectInstrument(_instrumentIdx);
            _instrumentHeaderList[_instrumentIdx].SetSelected(true);
        }
    }
}
