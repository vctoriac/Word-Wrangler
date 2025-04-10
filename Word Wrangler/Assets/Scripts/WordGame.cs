using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WordGame : MonoBehaviour
{
    public TMP_Text wordText;
    public TMP_InputField inputField;
    public TMP_Text timerText;
    public TMP_Text feedbackText;
    public GameObject gameOverScreen;
    public TMP_Text scoreText;
    public Slider playerHealth;
    public Slider enemyHealth;
    public Animator enemyAnimator;
    public GameObject countdownPanel;  // Reference to the Countdown Panel
    public TMP_Text countdownText;     // Reference to the TMP_Text for countdown

    private string currentWord;
    private List<string> currentSynonyms;
    private HashSet<string> matchedSynonyms = new HashSet<string>();  // Track matched synonyms
    private float timeLeft = 30f;
    private bool gameRunning = false;
    private int score = 0;
    private int playerHP = 5;
    private int enemyHP = 5;

    void Start()
    {
        gameOverScreen.SetActive(false);
        feedbackText.text = "";
        UpdateScoreText();
        playerHealth.maxValue = playerHP;
        enemyHealth.maxValue = enemyHP;
        playerHealth.value = playerHP;
        enemyHealth.value = enemyHP;

        inputField.onSubmit.AddListener(CheckInput);

        // Start the countdown before allowing the player to start typing
        countdownPanel.SetActive(true);
        countdownText.text = "3";
        StartCoroutine(StartCountdown());
    }

    void Update()
    {
        if (!gameRunning) return;

        timeLeft -= Time.deltaTime;

        // Convert timeLeft (float) to minutes and seconds
        int minutes = Mathf.FloorToInt(timeLeft / 60);
        int seconds = Mathf.FloorToInt(timeLeft % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (timeLeft <= 0)
        {
            EndGame();
        }
    }

    IEnumerator StartCountdown()
    {
        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();  // Update countdown text
            yield return new WaitForSeconds(1f);  // Wait for 1 second
        }

        countdownText.text = "GO!";  // Display "GO!" when countdown ends
        yield return new WaitForSeconds(1f);  // Wait for 1 second

        countdownPanel.SetActive(false);  // Hide the countdown panel after countdown finishes

        // Start the game
        gameRunning = true;
        inputField.interactable = true;  // Enable the input field
        inputField.ActivateInputField();  // Automatically focus on the input field so the player can start typing
        PickRandomWord();
    }


    void PickRandomWord()
    {
        var keys = new List<string>(WordBank.Words.Keys);
        currentWord = keys[Random.Range(0, keys.Count)];
        currentSynonyms = WordBank.Words[currentWord];
        matchedSynonyms.Clear();
        wordText.text = currentWord;
        timeLeft = 30f;
    }

    void CheckInput(string userInput)
    {
        if (!gameRunning) return;

        userInput = userInput.Trim().ToLower();

        if (currentSynonyms.Contains(userInput))
        {
            if (matchedSynonyms.Contains(userInput))
            {
                feedbackText.text = "That one's already been done!";
            }
            else
            {
                feedbackText.text = "Hit!";
                matchedSynonyms.Add(userInput);
                score += 10;
                enemyHealth.value -= 1;

                if (enemyHealth.value <= 0)
                {
                    EndGame(true);
                    return;
                }

                if (matchedSynonyms.Count == currentSynonyms.Count)
                {
                    NextRound();
                }
            }
        }
        else
        {
            feedbackText.text = "Miss!";
            score -= 5;
            EnemyShoots();
        }

        inputField.text = "";
        inputField.ActivateInputField();
        UpdateScoreText();
    }

    void EnemyShoots()
    {
        feedbackText.text += " The enemy fired!";
        playerHP--;
        playerHealth.value = playerHP;

        if (playerHP <= 0)
        {
            EndGame(false);
        }
    }

    void NextRound()
    {
        PickRandomWord();
    }

    void EndGame(bool won = false)
    {
        gameRunning = false;
        gameOverScreen.SetActive(true);

        if (won)
        {
            gameOverScreen.GetComponentInChildren<TMP_Text>().text = "You Won!";
        }
        else
        {
            gameOverScreen.GetComponentInChildren<TMP_Text>().text = "You Lost!";
        }
    }

    void UpdateScoreText()
    {
        scoreText.text = score.ToString();
    }
}
