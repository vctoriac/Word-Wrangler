using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerAnimation : MonoBehaviour
{
    public Image timerImage; // The UI Image with the sprite sheet
    public Sprite[] timerSprites; // The individual sprites for each frame
    public float totalTime = 30f; // Total time for the countdown (30 seconds)

    private void Update()
    {
        // Get the time left from the WordGame script
        float timeLeft = FindObjectOfType<WordGame>()?.TimeLeft ?? totalTime;

        // Calculate the elapsed time as a percentage of the total time
        float timePassed = totalTime - Mathf.Max(0, timeLeft);

        // Calculate which frame to show based on the time passed
        int frameIndex = Mathf.FloorToInt(timePassed / (totalTime / timerSprites.Length));

        // Update the sprite for the timer image based on the current frame index
        if (frameIndex >= 0 && frameIndex < timerSprites.Length)
        {
            timerImage.sprite = timerSprites[frameIndex];
        }
    }
}
