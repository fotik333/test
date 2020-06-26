using System;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Listeners
{
    public class SliderValueChangedListener : MonoBehaviour
    {
        private string _sliderColor;

        private OptionsManager _optionsManager;
        private Slider _slider;

        private void Start()
        {
            var game = GameManager.Instance;

            _sliderColor = gameObject.name.Remove(gameObject.name.IndexOf("Slider", StringComparison.Ordinal));
            _slider = GetComponent<Slider>();

            _optionsManager = game.OptionsManager;
            var value = _optionsManager.GetSingleColor(_sliderColor);

            if (Math.Abs(value) <= 1)
            {
                _slider.value = value;
            }

            _slider.onValueChanged.AddListener(ValueChanged);
        }

        private void ValueChanged(float value)
        {
            _optionsManager.AdjustColor(value, _sliderColor);
        }

        private void OnDestroy()
        {
            _slider.onValueChanged.RemoveListener(ValueChanged);
        }
    }
}