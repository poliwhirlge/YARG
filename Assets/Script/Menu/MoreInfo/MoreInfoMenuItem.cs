using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using YARG.Menu.Navigation;

namespace YARG.Menu.MoreInfo
{
    public class MoreInfoMenuItem : NavigatableBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _title;

        private bool _active;
        public bool Active
        {
            get => _active;
            set
            {
                _active = value;
            }
        }

        private Action<bool> _activeChangedCallback;

        public void Initialize(string title, bool active, Action<bool> activeChangedCallback)
        {
            _title.text = title;
            _activeChangedCallback = activeChangedCallback;

            Active = active;
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            Confirm();
        }

        public override void Confirm()
        {
            base.Confirm();

            Active = !Active;

            _activeChangedCallback?.Invoke(Active);
        }
    }
}