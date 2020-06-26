using System;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Listeners
{
    public class ButtonClickedListener : MonoBehaviour
    {
        private string _name;
        private SomeButtonClickedManager _handler;

        private void Start()
        {
            _name = gameObject.name.Remove(gameObject.name.IndexOf("Button", StringComparison.Ordinal));

            _handler = GameManager.Instance.SomeButtonClickedManager;

            GetComponent<Button>().onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            _handler.OnSomeButtonClicked(_name);
        }
    }
}
