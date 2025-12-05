using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour
{
    [Header("Gameplay")]
    public GameObject itemPrefab;
    public Transform conveyorStart;
    public Transform conveyorEnd;

    private List<RetailItem> currentItems = new List<RetailItem>();
    public int minItems = 1;
    public int maxItems = 9;

    [Header("Timer")]
    public float gameTime = 30f;
    private float timer;
    private bool gameRunning = false;

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public GameObject startMenu;
    public GameObject retryMenu;

    [Header("Sounds")]
    public AudioClip scanSound;
    public AudioClip receiptSound;
    private AudioSource audioSource;

    private int score = 0;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogWarning("AudioSource missing! Adding one automatically.");
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        timer = gameTime;
        UpdateTimerUI();

        
        startMenu.SetActive(true);
        retryMenu.SetActive(false);
        gameRunning = false;
    }

    public void StartGame()
    {
        // Reset values
        score = 0;
        scoreText.text = "Score: 0";
        timer = gameTime;
        gameRunning = true;

        
        startMenu.SetActive(false);
        retryMenu.SetActive(false);

        SpawnCustomer();
    }

    public void RetryGame()
    {
        StartGame();  
    }

    private void Update()
    {
        if (!gameRunning) return;

        timer -= Time.deltaTime;
        UpdateTimerUI();

        if (timer <= 0)
        {
            EndGame();
        }
    }

    private void UpdateTimerUI()
    {
        timerText.text = "Time: " + Mathf.Ceil(timer).ToString();
    }

    private void SpawnCustomer()
    {
        int itemCount = Random.Range(minItems, maxItems + 1);

        for (int i = 0; i < itemCount; i++)
        {
            Vector3 spawnPos = Vector3.Lerp(
                conveyorStart.position,
                conveyorEnd.position,
                Random.Range(0f, 1f)
            );

            spawnPos.x += Random.Range(-0.5f, 0.5f);

            GameObject newItem = Instantiate(itemPrefab, spawnPos, Quaternion.identity);
            RetailItem itemScript = newItem.GetComponent<RetailItem>();
            itemScript.controller = this;
            currentItems.Add(itemScript);
        }
    }

    public void ItemClicked(RetailItem item)
    {
        if (!gameRunning) return;

        currentItems.Remove(item);
        Destroy(item.gameObject);

        
        audioSource.PlayOneShot(scanSound);

        
        score++;
        scoreText.text = "Score: " + score;

        if (currentItems.Count == 0)
        {
            audioSource.PlayOneShot(receiptSound);
            SpawnCustomer();
        }
    }

    private void EndGame()
    {
        gameRunning = false;

        
        foreach (var item in currentItems)
        {
            if (item != null)
                Destroy(item.gameObject);
        }
        currentItems.Clear();

        retryMenu.SetActive(true);
    }
}
