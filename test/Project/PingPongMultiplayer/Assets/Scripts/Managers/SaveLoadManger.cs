using UnityEngine;

namespace Managers
{
    public class SaveLoadManger
    {
        private readonly GameManager _gameManager;

        private const string 
            RedColorKey = "RedColor",
            GreenColorKey = "GreenColor",
            BlueColorKey = "BlueColor",
            RecordKey = "Record";

        public SaveLoadManger()
        {
            _gameManager = GameManager.Instance;
        
            _gameManager.Record = PlayerPrefs.HasKey(RecordKey) ? PlayerPrefs.GetInt(RecordKey) : 0;
            _gameManager.OptionsManager.ColorUpdated += OnColorUpdated;
        
            var color = new Color
            {
                r = PlayerPrefs.HasKey(RedColorKey) ? PlayerPrefs.GetFloat(RedColorKey) : 1,
                g = PlayerPrefs.HasKey(GreenColorKey) ? PlayerPrefs.GetFloat(GreenColorKey) : 1,
                b = PlayerPrefs.HasKey(BlueColorKey) ? PlayerPrefs.GetFloat(BlueColorKey) : 1,
                a = 1
            };

            _gameManager.OptionsManager.SetColor(color);
        }

        public void SaveRecord()
        {
            PlayerPrefs.SetInt(RecordKey, _gameManager.Record);
        }

        private static void OnColorUpdated(object sender, Color color)
        {
            PlayerPrefs.SetFloat(RedColorKey, color.r);
            PlayerPrefs.SetFloat(GreenColorKey, color.g);
            PlayerPrefs.SetFloat(BlueColorKey, color.b);
        }
    }
}