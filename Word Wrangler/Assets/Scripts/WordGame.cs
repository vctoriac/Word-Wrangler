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
    public Slider playerHealth;
    public Slider enemyHealth;
    public Animator enemyAnimator;
    public GameObject countdownPanel;
    public TMP_Text countdownText;
    public GameObject pauseScreen;
    public Button pauseButton;   
    public Button resumeButton;
    public CharacterAnimatorUI playerShootAnimator;
    public CharacterAnimatorUI enemyShootAnimator;
    public GameObject speechBubble;



    public int synonymsToMatch = 2;
    public int grazeThreshold = 1;

    public Image timerImage;
    public Sprite[] timerSprites;
    public float frameInterval = 1f;
    public float feedbackDuration = 3f;

    private int timerFrame = 0;
    private float frameTimer = 0f;
    private Coroutine feedbackCoroutine;

    private string currentWord;
    private List<string> currentSynonyms;
    private HashSet<string> matchedSynonyms = new HashSet<string>();
    private List<string> unusedWords = new List<string>();
    private float timeLeft = 30f;
    public float TimeLeft => timeLeft;

    private bool gameRunning = false;
    private float playerHP = 5f;
    private int enemyHP = 5;

    void Start()
    {
        if (speechBubble != null)
            speechBubble.SetActive(false);

        Time.timeScale = 1f; // Ensure normal time on restart
        gameOverScreen.SetActive(false);
        feedbackText.text = "";

        playerHealth.maxValue = playerHP;
        enemyHealth.maxValue = enemyHP;
        playerHealth.value = playerHP;
        enemyHealth.value = enemyHP;

        

        inputField.onSubmit.AddListener(CheckInput);
        unusedWords = new List<string>(WordBank.Words.Keys);

        countdownPanel.SetActive(true);
        countdownText.text = "3";
        StartCoroutine(StartCountdown());

        pauseButton.onClick.AddListener(PauseGame);
        resumeButton.onClick.AddListener(ResumeGame);
        pauseScreen.SetActive(false);
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

        AnimateTimer();
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

    public void PauseGame()
    {
        if (!gameRunning) return;

        Time.timeScale = 0f;
        pauseScreen.SetActive(true);
    }

    public void ResumeGame()
    {
        pauseScreen.SetActive(false);
        Time.timeScale = 1f;

        PickRandomWord(); // Change to a new word
        inputField.text = "";
        inputField.ActivateInputField();
        inputField.selectionStringAnchorPosition = 0;
        inputField.selectionStringFocusPosition = 0;
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

        timerFrame = 0;
        frameTimer = 0f;

        if (timerSprites.Length > 0 && timerImage != null)
        {
            timerImage.sprite = timerSprites[timerFrame];
        }
    }

    void AnimateTimer()
    {
        frameTimer += Time.deltaTime;

        if (frameTimer >= frameInterval)
        {
            frameTimer = 0f;
            timerFrame++;

            if (timerFrame >= timerSprites.Length)
            {
                timerFrame = 0; // Loop
            }

            if (timerFrame < timerSprites.Length && timerImage != null)
            {
                timerImage.sprite = timerSprites[timerFrame];
            }
        }
    }

    void CheckInput(string userInput)
    {
        if (!gameRunning) return;

        userInput = userInput.Trim().ToLower();

        if (currentSynonyms.Contains(userInput))
        {
            if (matchedSynonyms.Contains(userInput))
            {
                ShowFeedback("That's already been done!");
            }
            else
            {
                ShowFeedback("Hit!");
                matchedSynonyms.Add(userInput);
                enemyHealth.value -= 1;

                playerShootAnimator?.PlayShootAnimation(); 

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
            ShowFeedback("Miss!");
            EnemyShoots(); 
        }


        inputField.text = "";
        inputField.ActivateInputField();
        inputField.selectionStringAnchorPosition = 0;
        inputField.selectionStringFocusPosition = 0;
    }

    void EnemyShoots()
    {
        playerHP -= 1f;
        playerHealth.value = playerHP;

        enemyShootAnimator?.PlayShootAnimation(); // Trigger enemy shooting animation

        if (playerHP <= 0)
        {
            EndGame(false);
        }
    }


    public void SkipWord()
    {
        if (!gameRunning) return;

        enemyShootAnimator?.PlayShootAnimation(); // Trigger enemy shooting animation

        if (matchedSynonyms.Count >= grazeThreshold)
        {
            ShowFeedback("Skipped! The enemy grazed me!");
            playerHP -= 0.5f;
        }
        else
        {
            ShowFeedback("Skipped! The enemy fired!");
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

    void ShowFeedback(string message)
    {
        if (feedbackCoroutine != null)
            StopCoroutine(feedbackCoroutine);

        feedbackCoroutine = StartCoroutine(FeedbackRoutine(message));
    }

    IEnumerator FeedbackRoutine(string message)
    {
        feedbackText.text = message;
        if (speechBubble != null)
            speechBubble.SetActive(true);

        yield return new WaitForSeconds(feedbackDuration);

        feedbackText.text = "";
        if (speechBubble != null)
            speechBubble.SetActive(false);
    }

}
