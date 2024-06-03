﻿using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using YARG.Core.Input;
using YARG.Menu.Data;
using YARG.Menu.Navigation;
using YARG.Core.Logging;

namespace YARG.Menu.Persistent
{
    public class HelpBarButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField]
        private Image _buttonImage;
        [SerializeField]
        private Image _buttonBackground;
        [SerializeField]
        private Image _buttonOutline;

        [SerializeField]
        private Button _button;

        [SerializeField]
        private TextMeshProUGUI _buttonLabel;
        [SerializeField]
        private TextMeshProUGUI _buttonText;

        public enum HelpButtonStyle
        {
            Default,
            Filled
        }

        [SerializeField]
        private HelpButtonStyle _buttonStyle;

        private NavigationScheme.Entry? _entry;

        private Color _buttonBackgroundColor;
        private Color _buttonImageColor;
        private Color _buttonBackgroundColorOnHover;

        public void SetInfoFromSchemeEntry(NavigationScheme.Entry entry)
        {
            _entry = entry;
            var icons = MenuData.NavigationIcons;
            _buttonBackgroundColor = icons.GetColor(entry.Action);
            _buttonBackgroundColor.a = 0.2f;
            _buttonBackgroundColorOnHover = icons.GetColor(entry.Action);
            _buttonBackgroundColorOnHover.a = 0.4f;
            _buttonImageColor = icons.GetColor(entry.Action);
            _buttonImageColor.a = 1f;

            // Label
            _buttonLabel.text = entry.DisplayName;
            _buttonLabel.color = Color.white;

            // Show/hide text and transitions
            var special = entry.Action is MenuAction.Select or MenuAction.Start;
            _buttonText.gameObject.SetActive(!special);
            _button.transition = special
                ? Selectable.Transition.None
                : Selectable.Transition.SpriteSwap;

            // Set colors
            _buttonImage.sprite = icons.GetIcon(entry.Action);
            _buttonImage.color = _buttonImageColor;
            _buttonText.color = Color.white;
            if (_buttonStyle == HelpButtonStyle.Default)
            {
                _buttonBackground.color = Color.clear;
                _buttonOutline.color = Color.clear;
            }
            else
            {
                _buttonBackground.color = _buttonBackgroundColor;
                _buttonOutline.color = _buttonBackgroundColor;
            }
            
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            YargLogger.LogFormatInfo("{0} pointer enter", _entry?.Action);
            _buttonBackground.color = _buttonBackgroundColor;
            _buttonImage.color = _buttonImageColor;
            _buttonLabel.color = Color.white;
            _buttonText.color = Color.white;
            _buttonOutline.color = _buttonBackgroundColor;
            if (_buttonStyle == HelpButtonStyle.Default)
            {
                _buttonBackground.color = _buttonBackgroundColor;
                _buttonOutline.color = _buttonBackgroundColor;
            }
            else
            {
                _buttonBackground.color = _buttonBackgroundColorOnHover;
                _buttonOutline.color = _buttonBackgroundColorOnHover;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _buttonBackground.color = Color.clear;
            _buttonImage.color = _buttonImageColor;
            _buttonLabel.color = Color.white;
            _buttonText.color = Color.white;
            _buttonOutline.color = Color.clear;
            if (_buttonStyle == HelpButtonStyle.Default)
            {
                _buttonBackground.color = Color.clear;
                _buttonOutline.color = Color.clear;
            }
            else
            {
                _buttonBackground.color = _buttonBackgroundColor;
                _buttonOutline.color = _buttonBackgroundColor;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _buttonBackground.color = Color.grey;
            _buttonImage.color = Color.grey;
            _buttonLabel.color = Color.grey;
            _buttonText.color = Color.grey;
            _buttonOutline.color = Color.grey;

            _entry?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _buttonBackground.color = Color.clear;
            _buttonImage.color = _buttonImageColor;
            _buttonLabel.color = Color.white;
            _buttonText.color = Color.white;
            _buttonOutline.color = Color.clear;
            if (_buttonStyle == HelpButtonStyle.Default)
            {
                _buttonBackground.color = Color.clear;
                _buttonOutline.color = Color.clear;
            }
            else
            {
                _buttonBackground.color = _buttonBackgroundColor;
                _buttonOutline.color = _buttonBackgroundColor;
            }

            _entry?.InvokeHoldOffHandler();
        }
    }
}