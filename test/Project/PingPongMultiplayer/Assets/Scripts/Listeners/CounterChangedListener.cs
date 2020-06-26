using Interfaces;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Listeners
{
    public class CounterChangedListener : MonoBehaviour, IValueChangedListener
    {
        private GamePlayManager _gamePlayManager;
        private Text _text;
    
        private void Start()
        {
            _gamePlayManager = GameManager.Instance.CurrentGamePlayManager as GamePlayManager;
            _text = GetComponent<Text>();
            _gamePlayManager.CounterUpdated += OnValueChanged;
        }

        public void OnValueChanged(object sender, int value)
        {
            _text.text = value.ToString();
        }

        private void OnDestroy()
        {
            _gamePlayManager.CounterUpdated -= OnValueChanged;
        }
    }
}
