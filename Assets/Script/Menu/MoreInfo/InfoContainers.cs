using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using YARG.Core;
using YARG.Core.Input;
using YARG.Core.Logging;
using YARG.Core.Song;
using YARG.Menu;
using YARG.Menu.MusicLibrary;
using YARG.Menu.Navigation;
using YARG.Scores;
using YARG.Song;

namespace YARG.Menu.MoreInfo
{
    public class InfoContainers : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _genre;
        [SerializeField]
        private TextMeshProUGUI _charter;
        [SerializeField]
        private TextMeshProUGUI _year;
        [SerializeField]
        private TextMeshProUGUI _length;

        [Space]
        [SerializeField]
        private GameObject _difficultyRingPrefab;
        [SerializeField]
        private Transform _basicDiffultyRings;
        [SerializeField]
        private Transform _proDiffultyRings;

        [Space]
        [SerializeField]
        private GameObject _scoreViewPrefab;
        [SerializeField]
        private Transform _basicScores;
        [SerializeField]
        private Transform _proScores;

        private readonly List<DifficultyRing> _difficultyRings = new();
        private readonly List<ScoreView> _scoreViews = new();
        
        private List<PlayerScoreRecord> _displayScores;

        private bool _initialized = false;
        private int _instrumentIdx = 0;

        public void Initialize()
        {
            for (int i = 0; i < 5; ++i)
            {
                var go = Instantiate(_difficultyRingPrefab, _basicDiffultyRings);
                _difficultyRings.Add(go.GetComponent<DifficultyRing>());
            }

            for (int i = 0; i < 5; ++i)
            {
                var go = Instantiate(_difficultyRingPrefab, _proDiffultyRings);
                _difficultyRings.Add(go.GetComponent<DifficultyRing>());
            }

            for (int i = 0; i < 5; ++i)
            {
                var go = Instantiate(_scoreViewPrefab, _basicScores);
                _scoreViews.Add(go.GetComponent<ScoreView>());
            }

            for (int i = 0; i < 5; ++i)
            {
                var go = Instantiate(_scoreViewPrefab, _proScores);
                _scoreViews.Add(go.GetComponent<ScoreView>());
            }

            foreach (var scoreView in _scoreViews)
            {
                scoreView.Reset();
            }

            _initialized = true;
        }

        public void Awake()
        {
        }

        private void OnEnable()
        {
            _genre.text = GlobalVariables.State.CurrentSong.Genre;
            _year.text = GlobalVariables.State.CurrentSong.Year;
            _charter.text = GlobalVariables.State.CurrentSong.Charter.Str;
            var x = GlobalVariables.State.CurrentSong;
            
            var time = TimeSpan.FromMilliseconds(GlobalVariables.State.CurrentSong.SongLengthMilliseconds);
            if (time.Hours > 0)
            {
                _length.text = time.ToString(@"h\:mm\:ss");
            }
            else
            {
                _length.text = time.ToString(@"m\:ss");
            }

            if (!_initialized)
            {
                Initialize();
            }

            var entry = GlobalVariables.State.CurrentSong;
            _difficultyRings[0].SetInfo("guitar", "FiveFretGuitar", entry[Instrument.FiveFretGuitar]);
            _difficultyRings[1].SetInfo("bass", "FiveFretBass", entry[Instrument.FiveFretBass]);
            if (entry.HasInstrument(Instrument.FiveLaneDrums))
            {
                _difficultyRings[2].SetInfo("ghDrums", "FiveLaneDrums", entry[Instrument.FiveLaneDrums]);
            }
            else
            {
                _difficultyRings[2].SetInfo("drums", "FourLaneDrums", entry[Instrument.FourLaneDrums]);
            }

            _difficultyRings[3].SetInfo("keys", "Keys", entry[Instrument.Keys]);

            if (entry.HasInstrument(Instrument.Harmony))
            {
                _difficultyRings[4].SetInfo(
                    entry.VocalsCount switch
                    {
                        2 => "twoVocals",
                        >= 3 => "harmVocals",
                        _ => "vocals"
                    },
                    "Harmony",
                    entry[Instrument.Harmony]
                );
            }
            else
            {
                _difficultyRings[4].SetInfo("vocals", "Vocals", entry[Instrument.Vocals]);
            }

            if (entry.HasInstrument(Instrument.ProGuitar_17Fret) || entry.HasInstrument(Instrument.ProGuitar_22Fret))
            {
                var values = entry[Instrument.ProGuitar_17Fret];
                if (values.Intensity == -1)
                    values = entry[Instrument.ProGuitar_22Fret];
                _difficultyRings[5].SetInfo("realGuitar", "ProGuitar", values);
            }

            if (entry.HasInstrument(Instrument.ProBass_17Fret) || entry.HasInstrument(Instrument.ProBass_22Fret))
            {
                var values = entry[Instrument.ProBass_17Fret];
                if (values.Intensity == -1)
                    values = entry[Instrument.ProBass_22Fret];
                _difficultyRings[6].SetInfo("realBass", "ProBass", values);
            }

            _difficultyRings[7].SetInfo("realDrums", "ProDrums", entry[Instrument.ProDrums]);
            _difficultyRings[8].SetInfo("realKeys", "ProKeys", entry[Instrument.ProKeys]);
            _difficultyRings[9].SetInfo("trueDrums", "TrueDrums", PartValues.Default);

            _displayScores = ScoreContainer.GetHighScoresByInstrumentAndDifficulty(entry.Hash);
            //UpdateScores();
        }

        private void ClearScores()
        {
            foreach (var scoreView in _scoreViews)
            {
                scoreView.Reset();
            }
        }

        private void UpdateScores(int index)
        {
            ClearScores();
            var instruments = new List<Instrument> {Instrument.FiveFretGuitar, Instrument.FiveFretBass, Instrument.FourLaneDrums, Instrument.Keys, Instrument.Vocals};
            foreach (var score in _displayScores)
            {
                if (score.Instrument == instruments[_instrumentIdx])
                {
                    _scoreViews[(int) score.Difficulty].SetScore(score);
                }
            }
        }

        public void SelectInstrument(int index)
        {
            _instrumentIdx = index;
            UpdateScores(index);
        }
    }


}