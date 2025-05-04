using UnityEngine;
using UnityEngine.UI;

public class ScreenSwitcher : MonoBehaviour
{
    public GameObject[] panels;
    public GameObject nextButton;
    public GameObject previousButton;

    private int currentPanelIndex = 0;

    void Start()
    {
        ShowPanel(currentPanelIndex);
    }

    public void ShowNextPanel()
    {
        if (currentPanelIndex < panels.Length - 1)
        {
            currentPanelIndex++;
            ShowPanel(currentPanelIndex);
        }
    }

    public void ShowPreviousPanel()
    {
        if (currentPanelIndex > 0)
        {
            currentPanelIndex--;
            ShowPanel(currentPanelIndex);
        }
    }

    private void ShowPanel(int index)
    {
        // Hide all panels
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(false);
        }

        // Show current panel
        panels[index].SetActive(true);

        // Toggle button visibility
        if (nextButton != null)
            nextButton.SetActive(index < panels.Length - 1);

        if (previousButton != null)
            previousButton.SetActive(index > 0);
    }
}
