using Core.Assignment03;
using System;
using UnityEngine.UIElements;

namespace Core.Assignment04
{
    public class ScreenShotElementView : VisualElement, ISelectable
    {
        public static event Action<string> SelectionChanged;

        public string ScreenShotName { get; private set; }

        public Selector Selector { get; private set; }

        public ScreenShotElementView(string screenShotName)
        {
            ScreenShotName = screenShotName;

            name = screenShotName;
            AddToClassList("screenshot-preview");
            Selector = new Selector(this);
            this.AddManipulator(Selector);

            UpdateContent();
        }

        /// <summary>
        /// Update visual element background image
        /// </summary>
        public void UpdateContent()
        {
            style.backgroundImage = new StyleBackground(ScreenShotEditor.LoadTexture(ScreenShotName));
        }

        public void Selected()
        {
            SelectionChanged?.Invoke(ScreenShotName);
        }

        public void Unselected()
        {

        }
    }
}