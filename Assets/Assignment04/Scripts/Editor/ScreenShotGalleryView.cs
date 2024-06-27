using UnityEngine.UIElements;
using UnityEditor;

namespace Core.Assignment04
{
    public class ScreenShotGalleryView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<ScreenShotGalleryView, UxmlTraits> { }

        VisualElement grid;

        public ScreenShotGalleryView()
        {
            VisualTreeAsset visualTreeAsset = EditorGUIUtility.Load("Assets/Assignment04/Styles/ScreenshotTool/screenshot-gallery-view.uxml") as VisualTreeAsset;
            visualTreeAsset.CloneTree(this);

            grid = this.Q("gallery-grid");

            SetGallery();

            EditorApplication.playModeStateChanged += (e) => SetGallery();
        }        

        /// <summary>
        /// Set gallery with all screenshots
        /// </summary>
        private void SetGallery()
        {
            grid.Clear();

            foreach (string file in ScreenShotEditor.ScreenShots)
            {
                grid.Add(new ScreenShotElementView(ScreenShotEditor.GetScreenShotName(file)));
            }
        }

        /// <summary>
        /// Select the first screenshot in gallery
        /// </summary>
        public void SelectFirst()
        {
            grid.Q<ScreenShotElementView>().Selector.Select();
        }

        /// <summary>
        /// Add a new screenshot in gallery
        /// </summary>
        /// <param name="screenShotName">screenshot name</param>
        /// <param name="keepOriginalFile">if it has to keep the old one</param>
        public void AddNewScreenShot(string screenShotName, bool keepOriginalFile)
        {
            if (keepOriginalFile) // if the old one has to be kept
            { // add a new screenshot to gallery
                grid.Add(new ScreenShotElementView(ScreenShotEditor.GetScreenShotName(screenShotName)));
                return;
            }
            // update the screenshot with same name otherwise
            this.Q<ScreenShotElementView>(screenShotName).UpdateContent();
        }
    }
}