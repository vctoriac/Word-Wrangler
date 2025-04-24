using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterAnimatorUI : MonoBehaviour
{
    public Image characterImage;               // UI Image component
    public List<Sprite> shootFrames;           // List of sprites for shooting animation
    public float frameRate = 0.1f;             // Time between frames

    private bool isAnimating = false;

    public void PlayShootAnimation()
    {
        if (!isAnimating)
        {
            StartCoroutine(PlayShootCoroutine());
        }
    }

    IEnumerator PlayShootCoroutine()
    {
        isAnimating = true;

        foreach (var frame in shootFrames)
        {
            characterImage.sprite = frame;
            yield return new WaitForSeconds(frameRate);
        }

        // Return to idle frame (assume first shootFrame is also idle)
        characterImage.sprite = shootFrames[0];
        isAnimating = false;
    }
}
