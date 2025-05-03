using UnityEngine;
using UnityEngine.UI;

public class ScreenSwitcher : MonoBehaviour
{
    // Reference to the panels
    public GameObject[] panels;

    // References to the back and forward arrow buttons
    public GameObject backArrow;
    public GameObject forwardArrow;

    // The current panel index
    private int currentPanelIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Set the first panel active
        ShowPanel(currentPanelIndex);
    }

    // Method to show the next panel
    public void ShowNextPanel()
    {
        currentPanelIndex++;
        if (currentPanelIndex >= panels.Length) 
        {
            currentPanelIndex = 0; // Loop back to the first panel
        }
        ShowPanel(currentPanelIndex);
    }

    // Method to show the previous panel
    public void ShowPreviousPanel()
    {
        currentPanelIndex--;
        if (currentPanelIndex < 0)
        {
            currentPanelIndex = panels.Length - 1; // Loop back to the last panel
        }
        ShowPanel(currentPanelIndex);
    }

    // Method to show a specific panel based on the index
    private void ShowPanel(int index)
    {
        // Deactivate all panels
        foreach (GameObject panel in panels)
        {
            panel.SetActive(false);
        }

        // Activate the selected panel
        panels[index].SetActive(true);

        // Control the visibility of the back and forward arrows
        UpdateArrowVisibility();
    }

    // Method to update the visibility of the arrows based on the current panel
    private void UpdateArrowVisibility()
    {
        // Hide back arrow on the first screen
        backArrow.SetActive(currentPanelIndex != 0);

        // Hide forward arrow on the last screen
        forwardArrow.SetActive(currentPanelIndex != panels.Length - 1);
    }
}
