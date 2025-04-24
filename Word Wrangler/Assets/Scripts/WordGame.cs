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
    public GameObject countdownPanel;
    public TMP_Text countdownText;

    public int synonymsToMatch = 2; // How many synonyms to pass the round
    public int grazeThreshold = 1;  // How many to avoid full damage on skip

    private string currentWord;
    private List<string> currentSynonyms;
    private HashSet<string> matchedSynonyms = new HashSet<string>();
    private List<string> unusedWords = new List<string>();
    private float timeLeft = 30f;
    public float TimeLeft => timeLeft;

    private bool gameRunning = false;
    private int score = 0;
    private float playerHP = 5f; // Float to allow 0.5 HP loss
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

        unusedWords = new List<string>(WordBank.Words.Keys);

        countdownPanel.SetActive(true);
        countdownText.text = "3";
        StartCoroutine(StartCountdown());
    }

    void Update()
    {
        if (!gameRunning) return;

        timeLeft -= Time.deltaTime;

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
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        countdownText.text = "GO!";
        yield return new WaitForSeconds(1f);

        countdownPanel.SetActive(false);

        gameRunning = true;
        inputField.interactable = true;
        inputField.ActivateInputField();
        PickRandomWord();
    }

    void PickRandomWord()
    {
        if (unusedWords.Count == 0)
        {
            unusedWords = new List<string>(WordBank.Words.Keys);
        }

        int randomIndex = Random.Range(0, unusedWords.Count);
        currentWord = unusedWords[randomIndex];
        unusedWords.RemoveAt(randomIndex);

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
                feedbackText.text = "That's already been done!";
            }
            else
            {
                feedbackText.text = "Hit!";
                matchedSynonyms.Add(userInput);
                score += 10;
                enemyHealth.value -= 1;

                FindObjectOfType<CharacterAnimatorUI>()?.PlayShootAnimation();

                if (enemyHealth.value <= 0)
                {
                    EndGame(true);
                    return;
                }

                if (matchedSynonyms.Count >= synonymsToMatch)
                {
                    NextRound();
                    return;
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
        inputField.selectionStringAnchorPosition = 0;
        inputField.selectionStringFocusPosition = 0;
        UpdateScoreText();
    }

    void EnemyShoots()
    {
        feedbackText.text += " The enemy fired!";
        playerHP -= 1f;
        playerHealth.value = playerHP;

        if (playerHP <= 0)
        {
            EndGame(false);
        }
    }

    public void SkipWord()
    {
        if (!gameRunning) return;

        if (matchedSynonyms.Count >= grazeThreshold)
        {
            feedbackText.text = "Skipped! The enemy grazed you!";
            playerHP -= 0.5f;
        }
        else
        {
            feedbackText.text = "Skipped! The enemy fired!";
            playerHP -= 1f;
        }

        playerHealth.value = playerHP;

        if (playerHP <= 0)
        {
            EndGame(false);
            return;
        }

        NextRound();
    }

    void NextRound()
    {
        PickRandomWord();
        inputField.text = "";
        inputField.ActivateInputField();
        inputField.selectionStringAnchorPosition = 0;
        inputField.selectionStringFocusPosition = 0;
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
