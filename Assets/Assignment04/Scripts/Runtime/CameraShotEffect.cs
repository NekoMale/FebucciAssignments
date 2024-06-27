using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Core.Assignment04
{
    public class CameraShotEffectUI : MonoBehaviour
    {
        [SerializeField] UIDocument linkedUI;
        /// <summary>
        /// Camera shot effect duration in seconds
        /// </summary>
        [Tooltip("Change this value only out of play mode")]
        [SerializeField] float _shotEffectSeconds = 0.1f;

        VisualElement cameraShotElement;

        private void Start()
        {
            cameraShotElement = linkedUI.rootVisualElement.Q("camera-shot");

            // Set transition property with custom duration value set by inspector
            cameraShotElement.style.transitionProperty = new List<StylePropertyName> { new StylePropertyName("background-color") };
            cameraShotElement.style.transitionTimingFunction = new List<EasingFunction> { new EasingFunction(EasingMode.EaseInCubic) };
            cameraShotElement.style.transitionDuration = new List<TimeValue> { new TimeValue(_shotEffectSeconds, TimeUnit.Second) };

            // Register to TakeScreenShotButton click event
            ScreenShotTakerUI.instance.TakeScreenShotButton.clicked += StartPlayEffect;
        }        

        private void StartPlayEffect()
        {
            StartCoroutine(PlayEffect());
        }

        private IEnumerator PlayEffect()
        {
            ScreenShotTakerUI.instance.TakeScreenShotButton.clicked -= StartPlayEffect;

            cameraShotElement.style.backgroundColor = Color.black;

            yield return new WaitForSeconds(_shotEffectSeconds);

            cameraShotElement.style.backgroundColor = Color.clear;

            // Register to TakeScreenShotButton click event cause when disabled it lose any reference
            ScreenShotTakerUI.instance.TakeScreenShotButton.clicked += StartPlayEffect;
        }
    }
}