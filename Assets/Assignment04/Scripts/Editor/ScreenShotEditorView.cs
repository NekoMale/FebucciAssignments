using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Core.Assignment04
{
    public class ScreenShotEditorView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<ScreenShotEditorView, UxmlTraits> { }

        public Button SaveButton { get; private set; }
        public event Action<string, Texture2D, bool> SaveButtonClicked;
        Toggle flipXToggle, flipYToggle, colorInvertToggle;

        string selectedFile;
        Texture2D currentTexture;

        public ScreenShotEditorView()
        {
            VisualTreeAsset visualTreeAsset = EditorGUIUtility.Load("Assets/Assignment04/Styles/ScreenshotTool/screenshot-editor-view.uxml") as VisualTreeAsset;
            visualTreeAsset.CloneTree(this);

            // Caching frequently used UI elements
            SaveButton = this.Q<Button>("save-button");
            flipXToggle = this.Q<Toggle>("flip-x-toggle");
            flipYToggle = this.Q<Toggle>("flip-y-toggle");
            colorInvertToggle = this.Q<Toggle>("color-invert-toggle");

            flipXToggle.RegisterValueChangedCallback(x => FlipX());
            flipYToggle.RegisterValueChangedCallback(x => FlipY());
            colorInvertToggle.RegisterValueChangedCallback(x => ColorInvert());

            SaveButton.clicked += () =>
            {
                // Invoke button click event
                SaveButtonClicked?.Invoke(selectedFile, currentTexture, this.Q<Toggle>("keep-original-toggle").value);
                // Select new screenshot in editor view
                OnScreenShotSelected(selectedFile);
            };

            ScreenShotElementView.SelectionChanged += OnScreenShotSelected;
        }

        private void ResetToggles()
        {
            flipXToggle.SetValueWithoutNotify(false);
            flipYToggle.SetValueWithoutNotify(false);
            colorInvertToggle.SetValueWithoutNotify(false);
        }

        /// <summary>
        /// On screenshot selection set editor view with new selected screenshot image
        /// and reset toggles
        /// </summary>
        /// <param name="screenShotName"></param>
        private void OnScreenShotSelected(string screenShotName)
        {
            ResetToggles();
            selectedFile = screenShotName;
            currentTexture = ScreenShotEditor.LoadTexture(selectedFile);
            this.Q("screenshot-preview").style.backgroundImage = new StyleBackground(currentTexture);
        }

        /// <summary>
        /// Flip texture's pixels on vertical axis
        /// </summary>
        private void FlipX()
        {
            Color[] colors = currentTexture.GetPixels();
            Color[] newColors = new Color[colors.Length];

            for (int x = 0; x < currentTexture.width; x++)
            {
                for (int y = 0; y < currentTexture.height; y++)
                {
                    newColors[x + y * currentTexture.width] = colors[(currentTexture.width - x - 1) + y * currentTexture.width];
                }
            }

            currentTexture.SetPixels(newColors);
            currentTexture.Apply();
        }

        /// <summary>
        /// Flip texture's pixels on horizontal axis
        /// </summary>
        private void FlipY()
        {
            Color[] colors = currentTexture.GetPixels();
            Color[] newColors = new Color[colors.Length];

            for (int x = 0; x < currentTexture.width; x++)
            {
                for (int y = 0; y < currentTexture.height; y++)
                {
                    newColors[x + y * currentTexture.width] = colors[x + (currentTexture.height - y - 1) * currentTexture.width];
                }
            }

            currentTexture.SetPixels(newColors);
            currentTexture.Apply();
        }

        /// <summary>
        /// Invert every texture pixel color
        /// </summary>
        private void ColorInvert()
        {
            Color[] colors = currentTexture.GetPixels();

            for (int x = 0; x < colors.Length; x++)
            {
                colors[x] = new Color(1f - colors[x].r, 1f - colors[x].g, 1f - colors[x].b, 1f);
            }

            currentTexture.SetPixels(colors);
            currentTexture.Apply();
        }
    }
}