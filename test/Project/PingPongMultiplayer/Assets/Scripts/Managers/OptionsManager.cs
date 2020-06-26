using System;
using UnityEngine;

namespace Managers
{
    public class OptionsManager
    {
        public float Red { get; private set; }
        public float Green { get; private set; }
        public float Blue { get; private set; }

        public event EventHandler<Color> ColorUpdated;

        public OptionsManager()
        {
            Red = Green = Blue = 1;
        }

        public Color GetColor()
        {
            return new Color {r = Red, g = Green, b = Blue, a = 1};
        }

        public void SetColor(Color color)
        {
            Red = color.r;
            Green = color.g;
            Blue = color.b;

            ColorUpdated?.Invoke(this, GetColor());
        }

        public float GetSingleColor(string color)
        {
            var colorInfo = GetType().GetProperty(color);

            if (colorInfo != null) return (float)colorInfo.GetValue(this);
        
            Debug.LogErrorFormat("GetColor: invalid color name: {0}", color);
            return 1;

        }

        public void AdjustColor(float value, string color)
        {
            var colorInfo = GetType().GetProperty(color);
        
            if (colorInfo == null)
            {
                Debug.LogErrorFormat("AdjustColor: invalid color name: {0}", color);
                return;
            }

            colorInfo.SetValue(this, value);
            ColorUpdated?.Invoke(this, GetColor());
        }
    }
}
