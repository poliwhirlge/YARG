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
        private TextMeshProUGUI _charterText;
        [SerializeField]
        private TextMeshProUGUI _contentRatingText;
        [SerializeField]
        private TextMeshProUGUI _playCountText;
        [SerializeField]
        private TextMeshProUGUI _fcCountText;
        [SerializeField]
        private TextMeshProUGUI _loadingPhraseText;
        [SerializeField]
        private Image _contentRatingIcon;
        [SerializeField]
        private Image _sourceIcon;
        [SerializeField]
        private Image _charterIcon;
        [SerializeField]
        private Image _charterIconBackground;
        [SerializeField]
        private Image _bandBarImage;
        [SerializeField]
        private DifficultyRing[] _difficultyRings;

        // [SerializeField]
        // private Transform _difficultyRingsTopContainer;
        // [SerializeField]
        // private Transform _difficultyRingsBottomContainer;
        //
        // [SerializeField]
        // private TextMeshProUGUI _album;
        // [SerializeField]
        // private TextMeshProUGUI _source;
        // [SerializeField]
        // private TextMeshProUGUI _charter;
        // [SerializeField]
        // private TextMeshProUGUI _genre;
        // [SerializeField]
        // private TextMeshProUGUI _subgenre;
        // [SerializeField]
        // private TextMeshProUGUI _year;
        // [SerializeField]
        // private TextMeshProUGUI _length;
        // [SerializeField]
        // private RawImage _albumCover;
        // [SerializeField]
        // private RawImage _albumCoverSmall;
        // [SerializeField]
        // private Image _sourceBackground;
        // [SerializeField]
        // private HelpBarButton _playButton;
        //
        // [Space]
        // [SerializeField]
        // private HoverButton _favoriteButton;
        // [SerializeField]
        // private Image _favoriteButtonImage;
        //
        // [SerializeField]
        // private GameObject _sidebarContents;
        // [SerializeField]
        // private GameObject _difficultiesContainer;
        // [SerializeField]
        // private GameObject _difficultiesDisplay;
        // [SerializeField]
        // private GameObject _albumTitleContainer;
        // [SerializeField]
        // private GameObject _timeContainer;
        // [SerializeField]
        // private GameObject _sourceContainer;
        // [SerializeField]
        // private GameObject _charterContainer;
        // [SerializeField]
        // private GameObject _genreContainer;
        // [SerializeField]
        // private GameObject _genreSpacer;
        // [SerializeField]
        // private Image _contentRatingImage;
        // [SerializeField]
        // private Sprite[] _contentRatingIcons;
        //
        // [Space]
        // [SerializeField]
        // private GameObject _difficultyRingPrefab;
    }
}
