using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using YARG.Core;
using YARG.Core.Chart;
using YARG.Core.Game;
using YARG.Core.Logging;
using YARG.Menu.MusicLibrary;

namespace YARG
{
    public class UnisonNotification : MonoBehaviour
    {

        [SerializeField]
        private TextMeshProUGUI _unisonNotificationText;

        [SerializeField]
        private Transform _unisonInstruments;

        [Space]
        [SerializeField]
        private GameObject _difficultyRingPrefab;

        private List<DifficultyRing> _difficultyRings = new();
        private List<GameObject> _instrumentIcons = new();
        private List<YargProfile> _playersInCurrentUnison = new();

        private Dictionary<Instrument, string> _instrumentToSpriteName = new()
        {
            { Instrument.ProDrums, "drums" },
            { Instrument.FiveFretGuitar, "guitar" },
            { Instrument.FiveFretBass, "bass" },
            { Instrument.FourLaneDrums, "drums" },
            { Instrument.Keys, "keys" },
        };

        // Start is called before the first frame update
        void Start()
        {
            gameObject.SetActive(false);
        }

        internal void OnUnisonPhraseStart(UnisonPhrase unisonPhrase, List<YargProfile> playersInUnison)
        {
            YargLogger.LogFormatInfo("Unison Phrase Start invoked! {0}", unisonPhrase.Phrase.Tick);
            _difficultyRings.Clear();

            _playersInCurrentUnison = playersInUnison;
            var diff = new Core.Song.PartValues
            {
                Intensity = 0
            };
            diff.SetSubtrack(0);
            foreach (var player in _playersInCurrentUnison)
            {
                var go = Instantiate(_difficultyRingPrefab, _unisonInstruments);
                var diffRing = go.GetComponent<DifficultyRing>();
                //diffRing.SetInfo("guitar", player.CurrentInstrument.ToString(), diff);
                var spriteName = _instrumentToSpriteName[player.CurrentInstrument] ?? "guitar";
                diffRing.SetParamsDirect(spriteName, 0.2f);
                _difficultyRings.Add(diffRing);
                _instrumentIcons.Add(go);
            }

            foreach (var inst in _difficultyRings)
            {
                inst.gameObject.SetActive(true);
            }
            gameObject.SetActive(true);
        }

        internal void OnUnisonPhraseFail(UnisonPhrase unisonPhrase, YargProfile yargProfile)
        {
            YargLogger.LogFormatInfo("Unison Phrase Fail invoked! {0} {1}", unisonPhrase.Phrase.Tick, yargProfile.CurrentInstrument);
            for (var i = 0; i < _playersInCurrentUnison.Count; i++)
            {
                if (_playersInCurrentUnison[i].Id == yargProfile.Id)
                {
                    var c = Color.red;
                    c.a = 0.2f;
                    _difficultyRings[i].SetColor(c);
                }
            }
        }

        internal void OnUnisonPhraseComplete(UnisonPhrase unisonPhrase, YargProfile yargProfile)
        {
            YargLogger.LogFormatInfo("Unison Phrase Complete invoked! {0} {1}", unisonPhrase.Phrase.Tick, yargProfile.CurrentInstrument);
            for (var i = 0; i < _playersInCurrentUnison.Count; i++)
            {
                if (_playersInCurrentUnison[i].Id == yargProfile.Id)
                {
                    var c = Color.white;
                    c.a = 1f;
                    _difficultyRings[i].SetColor(c);
                }
            }
        }

        IEnumerator Fade()
        {
            yield return new WaitForSeconds(1f);
            foreach(var go in _instrumentIcons)
            {
                go.SetActive(false);
                Destroy(go);
            }
            _instrumentIcons.Clear();
            gameObject.SetActive(false);
        }

        internal void OnUnisonPhraseEnd(UnisonPhrase unisonPhrase)
        {
            YargLogger.LogFormatInfo("Unison Phrase End invoked! {0}", unisonPhrase);

            StartCoroutine(Fade());
        }

        internal void OnUnisonPhraseAward(UnisonPhrase unisonPhrase)
        {
            YargLogger.LogFormatInfo("Unison Phrase Award invoked! {0}", unisonPhrase);
        }
    }
}
