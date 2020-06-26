using Managers;
using UnityEngine;

namespace Listeners
{
    public class SampleImageColorChangedListener : MonoBehaviour
    {
        private OptionsManager _optionsManager;
        private SpriteRenderer _renderer;

        private void Start()
        {
            _optionsManager = GameManager.Instance.OptionsManager;
            _renderer = GetComponent<SpriteRenderer>();
            _renderer.color = _optionsManager.GetColor();

            _optionsManager.ColorUpdated += OnColorUpdated;
        }

        private void OnColorUpdated(object sender, Color color)
        {
            _renderer.color = color;
        }

        private void OnDestroy()
        {
            _optionsManager.ColorUpdated -= OnColorUpdated;
        }
    }
}