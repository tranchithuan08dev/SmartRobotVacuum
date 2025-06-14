using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public RobotVacuumController robot;
    public Slider speedSlider;
    private CanvasGroup sliderCanvasGroup; // For fading/toggling visibility

    void Start()
    {
        if (speedSlider != null && robot != null)
        {
            speedSlider.value = robot.moveSpeed;
            speedSlider.onValueChanged.AddListener(UpdateSpeed);
            // Get or add CanvasGroup for visibility control
            sliderCanvasGroup = speedSlider.GetComponent<CanvasGroup>();
            if (sliderCanvasGroup == null)
            {
                sliderCanvasGroup = speedSlider.gameObject.AddComponent<CanvasGroup>();
            }
            sliderCanvasGroup.alpha = 1f; // Visible by default
            sliderCanvasGroup.interactable = true;
            sliderCanvasGroup.blocksRaycasts = true;
        }
        else
        {
            Debug.LogWarning("SpeedSlider or Robot not assigned in UIManager!");
        }
    }

    void Update()
    {
        // Toggle slider visibility with 'T' key
        if (Input.GetKeyDown(KeyCode.T))
        {
            ToggleSliderVisibility();
        }
    }

    void UpdateSpeed(float value)
    {
        if (robot != null)
        {
            robot.SetSpeed(value);
        }
    }

    void ToggleSliderVisibility()
    {
        if (sliderCanvasGroup != null)
        {
            bool isVisible = sliderCanvasGroup.alpha > 0f;
            sliderCanvasGroup.alpha = isVisible ? 0f : 1f;
            sliderCanvasGroup.interactable = !isVisible;
            sliderCanvasGroup.blocksRaycasts = !isVisible;
            Debug.Log($"Slider visibility: {!isVisible}");
        }
    }
}