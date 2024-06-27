using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

namespace Core.Assignment04
{
    public class ScreenShotTaker : MonoBehaviour
    {
        /// <summary>
        /// Zoom Factor
        /// </summary>
        [SerializeField] float zoomFactor = 1.0f;
        /// <summary>
        /// Zoom animation duration in seconds
        /// </summary>
        [SerializeField] float zoomSeconds = 0.25f;

        float startFOV; // stores camera starting FOV
        float deltaFOV, targetFOV; // caching values in order to make zoom animation effect

        Coroutine takingScreenShot;

        private void Start()
        {
            startFOV = Camera.main.fieldOfView; // cache starting FOV

            // Registering to sliders value change event
            ScreenShotTakerUI.instance.ZoomSlider.RegisterValueChangedCallback(v => SetZoomFactor(v.newValue));
            ScreenShotTakerUI.instance.TakeScreenShotButton.clicked += StartSavingScreenShot;

            // Set starting zoom factor
            SetZoomFactor(1f);
        }

        /// <summary>
        /// Set zoom factor editing camera FOV
        /// </summary>
        /// <param name="zoomFactor"></param>
        private void SetZoomFactor(float zoomFactor)
        {
            // Target FOV is startFOV divided by zoomFactor.
            // Smaller FOV, bigger zoom
            targetFOV = startFOV / zoomFactor; 
            // DeltaFOV is amount of FOV to change to complete animation
            deltaFOV = Mathf.Abs(targetFOV - Camera.main.fieldOfView);
        }

        /// <summary>
        /// Starts SaveScreenShot coroutine
        /// </summary>
        private void StartSavingScreenShot()
        {
            if (takingScreenShot != null) return;
            takingScreenShot = StartCoroutine(SaveScreenShot());
        }

        /// <summary>
        /// Takes screenshot disabling UI.<br></br>
        /// Screenshot name is curernt timestamp.
        /// </summary>
        private IEnumerator SaveScreenShot()
        {
            // Disables UI in order to remove it from ScreenShot
            ScreenShotTakerUI.instance.gameObject.SetActive(false);

            yield return new WaitForEndOfFrame(); // Wait for UI really disabled

            // Create directory to store screenshots if it doesn't exist
            if (!Directory.Exists(ScreenShotSettings.Path))
            {
                Directory.CreateDirectory(ScreenShotSettings.Path);
            }
            // Capture screenshot and raise event
            ScreenCapture.CaptureScreenshot(ScreenShotSettings.Path + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png");

            // Enable UI again and register again to sliders value change event
            ScreenShotTakerUI.instance.gameObject.SetActive(true);
            ScreenShotTakerUI.instance.ZoomSlider.RegisterValueChangedCallback(v => SetZoomFactor(v.newValue));
            ScreenShotTakerUI.instance.TakeScreenShotButton.clicked += StartSavingScreenShot;

            takingScreenShot = null;
        }

        void Update()
        {
            // Change camera FOV to target FOV in zoomSeconds
            Camera.main.fieldOfView = Mathf.MoveTowards(Camera.main.fieldOfView, targetFOV, deltaFOV / zoomSeconds * Time.deltaTime);
        }
    }
}