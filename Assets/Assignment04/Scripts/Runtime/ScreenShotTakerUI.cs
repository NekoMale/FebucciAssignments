using UnityEngine;
using UnityEngine.UIElements;

namespace Core.Assignment04
{
    public class ScreenShotTakerUI : MonoBehaviour
    {
        [SerializeField] UIDocument linkedUI;

        public static ScreenShotTakerUI instance { get; private set; }
        static float zoomSliderValue, gridSliderValue;

        VisualElement root, gridColumns, gridRows;
        public Slider ZoomSlider { get; private set; }
        public Button TakeScreenShotButton { get; private set; }
        public Slider gridSlider;
        Label zoomLabel;

        private void OnEnable()
        {
            // If there are 2 ScreenShotTakerUI
            if (instance != null && instance != this)
            {
                gameObject.SetActive(false); // Disable the second one
                Destroy(gameObject); // and destroy it
                return;
            }
            instance = this; // Set this as instance
            root = linkedUI.rootVisualElement; // cache linked DocumentUI root

            // Chacking used UI elements
            ZoomSlider = root.Q<Slider>("zoom-factor-slider");
            gridSlider = root.Q<Slider>("grid-slider");
            zoomLabel = root.Q<Label>("zoom-factor-value-label");
            gridColumns = root.Q("columns-container");
            gridRows = root.Q("rows-container");
            TakeScreenShotButton = root.Q<Button>("screenshot-button");

            // Set ratio label
            root.Q<Label>("aspect-ratio-value-label").text = ScreenShotSettings.RatioDisplay;

            // Register callback on zoom and grid changes
            ZoomSlider.RegisterValueChangedCallback(e => SetZoomLabel(e.newValue));
            gridSlider.RegisterValueChangedCallback(e => SetGrid(e.newValue));

            // If there were already values, set them
            // This is because if DocumentUI is disabled and re-enabled
            // UI loses old values and set default values again
            if (zoomSliderValue > 0 || gridSliderValue > 0)
            {
                ZoomSlider.value = zoomSliderValue;
                gridSlider.value = gridSliderValue;
                SetZoomLabel(zoomSliderValue);
            }
            else
            {
                // Set default zoom value
                SetZoomLabel(1f);
            }
        }

        private void OnDisable()
        {
            zoomSliderValue = ZoomSlider.value;
            gridSliderValue = gridSlider.value;
        }

        /// <summary>
        /// Set label with zoom value
        /// </summary>
        /// <param name="zoom"></param>
        private void SetZoomLabel(float value)
        {
            zoomLabel.text = value.ToString("#.0");
        }

        /// <summary>
        /// Change grid center size
        /// </summary>
        /// <param name="gridSize"></param>
        private void SetGrid(float value)
        {
            // column padding in % is half of parameter value 
            Length columnPadding = new Length(value * 50f, LengthUnit.Percent);
            // row padding in % is half of parameter value * inverse ratio
            Length rowPadding = new Length(value * 50f / ScreenShotSettings.Ratio, LengthUnit.Percent);

            gridColumns.style.paddingRight = new StyleLength(columnPadding);
            gridColumns.style.paddingLeft = new StyleLength(columnPadding);
            gridRows.style.paddingTop = new StyleLength(rowPadding);
            gridRows.style.paddingBottom = new StyleLength(rowPadding);
        }
    }
}