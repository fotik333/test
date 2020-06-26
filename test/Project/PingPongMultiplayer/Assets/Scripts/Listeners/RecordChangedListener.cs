using Interfaces;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Listeners
{
    public class RecordChangedListener : MonoBehaviour, IValueChangedListener
    {
        private GamePlayManager _gamePlayManager;
        private Text _text;
        private GameManager _gameManager;

        private void Start()
        {
            _gameManager = GameManager.Instance;
        
            _text = GetComponent<Text>();
            _text.text = "record: " + _gameManager.Record;
        
            _gamePlayManager = _gameManager.CurrentGamePlayManager as GamePlayManager;
            _gamePlayManager.RecordUpdated += OnValueChanged;
        }

        public void OnValueChanged(object o, int value)
        {
            _text.text = "record: " + value;
        }

        private void OnDestroy()
        {
            _gamePlayManager.RecordUpdated -= OnValueChanged;
        }
    }
}
