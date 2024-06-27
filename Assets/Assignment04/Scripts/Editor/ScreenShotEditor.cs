using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Core.Assignment04
{
    public class ScreenShotEditor : EditorWindow
    {
        VisualElement root;
        ScreenShotGalleryView galleryView;
        ScreenShotEditorView editorView;

        public static string[] ScreenShots => Directory.GetFiles(ScreenShotSettings.Path);

        [MenuItem("Assignments 4/ScreenShot Gallery")]
        public static void OpenScreenShotGallery()
        {
            GetWindow<ScreenShotEditor>();
        }

        private void CreateGUI()
        {
            root = rootVisualElement;

            VisualTreeAsset visualTreeAsset = EditorGUIUtility.Load("Assets/Assignment04/Styles/ScreenshotTool/screenshot-editor-window-view.uxml") as VisualTreeAsset;
            visualTreeAsset.CloneTree(root);

            galleryView = root.Q<ScreenShotGalleryView>();
            editorView = root.Q<ScreenShotEditorView>();

            galleryView.SelectFirst();

            editorView.SaveButtonClicked += SaveScreenShotAndUpdateGallery;
        }

        /// <summary>
        /// Return screenshot file name without path and png extension
        /// </summary>
        /// <param name="screenshotPath">screenshot file path with name and extension</param>
        /// <returns>screenshot file name without path and png extension</returns>
        public static string GetScreenShotName(string screenshotPath)
        {
            return screenshotPath.Substring(screenshotPath.LastIndexOf('/')).Replace(".png", "");
        }

        /// <summary>
        /// Load texture inside screenshot folder from file having same filename
        /// </summary>
        /// <param name="fileName">file name where take texture</param>
        /// <returns>loaded texture</returns>
        public static Texture2D LoadTexture(string fileName)
        {
            byte[] rawData = File.ReadAllBytes(ScreenShotSettings.Path + fileName + ".png");
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(rawData);
            return tex;
        }

        /// <summary>
        /// Save new screenshot and update gallery with the new one
        /// </summary>
        /// <param name="screenShotName">screenshot's file name to save</param>
        /// <param name="screenShotTexture">texture to save</param>
        /// <param name="keepOriginalFile">if original screenshot has to be kept</param>
        private void SaveScreenShotAndUpdateGallery(string screenShotName, Texture2D screenShotTexture, bool keepOriginalFile)
        {
            screenShotName = SaveScreenShot(screenShotName, screenShotTexture, keepOriginalFile);
            galleryView.AddNewScreenShot(screenShotName, keepOriginalFile);
        }

        /// <summary>
        /// Save edited screenshot
        /// </summary>
        /// <param name="screenShotName">original screenshot name</param>
        /// <param name="screenShotTexture">screenshot texture to save</param>
        /// <param name="keepOriginalFile">if original file has to be kept</param>
        /// <returns>New screenshot name</returns>
        private string SaveScreenShot(string screenShotName, Texture2D screenShotTexture, bool keepOriginalFile)
        {
            // Create screenshot directory if doesn't exist
            if (!Directory.Exists(ScreenShotSettings.Path))
            {
                Directory.CreateDirectory(ScreenShotSettings.Path);
            }
            byte[] itemBGBytes = screenShotTexture.EncodeToPNG();
            if (keepOriginalFile) // original file has to be kept
            { // rename screenshot adding a number equal to number of files with same prefix
                screenShotName += "_" + Directory.EnumerateFiles(ScreenShotSettings.Path).Count(fileName => fileName.Contains(screenShotName));
            }
            File.WriteAllBytes(ScreenShotSettings.Path + screenShotName + ".png", itemBGBytes);

            return screenShotName; // return the new screenshot name
        }

        private void OnGUI()
        {
            if (root == null) return;

            // update editor and gallery widths if resized
            // keeping editor size at 32.5% of window and between 350 and 700 pixels
            float width = Mathf.Max(350f, Mathf.Min(700f, root.resolvedStyle.width * 0.325f));

            editorView.style.width = width;
            editorView.style.minWidth = width;
            editorView.style.maxWidth = width;
            galleryView.style.width = root.resolvedStyle.width - width;
            galleryView.style.minWidth = root.resolvedStyle.width - width;
            galleryView.style.maxWidth = root.resolvedStyle.width - width;
        }
    }
}