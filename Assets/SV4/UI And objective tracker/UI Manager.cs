using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }


    [Header("Deliverys")]
    [SerializeField] private int maxDeliverys;

    [Header("ScoreSettings")]
    [SerializeField] private int scoreMultiplier = 5000;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI packagesCount;

    [SerializeField] private TextMeshProUGUI actionPromptText;
    [SerializeField] private Image waypointImage;               
    [SerializeField] private TextMeshProUGUI waypointDistanceText; 
    [SerializeField] private TextMeshProUGUI finalScoreText; 
    [SerializeField] private GameObject gameOverPanel; 


    [Header("References")]
    [SerializeField] private Transform droneTransform;  

    private float elapsedTime = 0f;
    public bool isRunning = false;
    private int Deliverys = 0;
   


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        if (WaypointManager.Instance != null)
        {
            UpdateWaypointUI();
        }
        if (isRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerUI();
        }
        CheckAllDeliveriesComplete();
    }

    private void UpdateWaypointUI()
    {
        var waypointManager = WaypointManager.Instance;

        // Determine the current waypoint (pickup or drop-off)
        Transform targetWaypoint = null;
       
        Sprite waypointSprite = null;

        if (waypointManager.CurrentPickupPoint?.IsActive == true)
        {
            targetWaypoint = waypointManager.CurrentPickupPoint.transform;
           
            waypointSprite = waypointManager.CurrentPickupPoint.PointImage;
        }
        else if (waypointManager.CurrentDropOffPoint?.IsActive == true)
        {
            targetWaypoint = waypointManager.CurrentDropOffPoint.transform;
           
            waypointSprite = waypointManager.CurrentDropOffPoint.PointImage;
        }

        // If there is an active waypoint, update the UI
        if (targetWaypoint != null)
        {
            float distance = Vector3.Distance(droneTransform.position, targetWaypoint.position);
            Vector3 screenPos = Camera.main.WorldToScreenPoint(targetWaypoint.position);
            if (Camera.main.pixelRect.Contains(screenPos) && screenPos.z >= 0)
            {
                waypointImage.transform.position = screenPos;
            }
            waypointDistanceText.text = $"{distance:F2}m";
            waypointImage.sprite = waypointSprite;
            waypointImage.enabled = true;
        }
        else
        {
            // No active waypoint

            waypointDistanceText.text = "";
            waypointImage.enabled = false;
        }
    }
    public void UpdateDeliverysCount()
    {
        Deliverys++;
        if (packagesCount != null)
        {
            packagesCount.text = $"Deliverys: {Deliverys}";
        }
    }
    public void StartTimer()
    {
        isRunning = true;
        elapsedTime = 0f; // Reset time when starting
        UpdateTimerUI();
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    private void UpdateTimerUI()
    {
        timerText.text = $"Time: {elapsedTime:F2}s";
    }
    public void UpdateWaypointUIElements(Sprite image, float distance)
    {
        waypointImage.sprite = image;
        waypointDistanceText.text = $"{distance:F2}m";
        waypointImage.enabled = true;
    }
    public void ShowActionPrompt(bool show, string message = "")
    {
        if (actionPromptText != null)
        {
            actionPromptText.gameObject.SetActive(show);
            actionPromptText.text = message;
        }
    }
    public void HideGameplayUI()
    {
        // Hide all UI elements except the end-game UI
        timerText.gameObject.SetActive(false);
        packagesCount.gameObject.SetActive(false);
        actionPromptText.gameObject.SetActive(false);
        waypointImage.gameObject.SetActive(false);
        waypointDistanceText.gameObject.SetActive(false);
    }

    public void ShowGameplayUI()
    {
        // Show all UI elements except the end-game UI
        timerText.gameObject.SetActive(true);
        packagesCount.gameObject.SetActive(true);
        actionPromptText.gameObject.SetActive(true);
        waypointImage.gameObject.SetActive(true);
        waypointDistanceText.gameObject.SetActive(true);
    }
    public void DisplayFinalScore()
    {
        isRunning = false; // Stop timer
        gameOverPanel.SetActive(true); // Show Game Over UI
        HideGameplayUI();
        // Prevent division by zero
        int finalScore = elapsedTime > 0 ? Mathf.RoundToInt((Mathf.Pow(Deliverys, 2) * scoreMultiplier) / elapsedTime) : 0;


        finalScoreText.text = $"Total Deliveries: {Deliverys}\nElapsed Time: {elapsedTime:F2} sec\nFinal Score: {finalScore:F2}";
        //Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }


    public void CheckAllDeliveriesComplete()
    {
        if (Deliverys >= maxDeliverys)
        {
            DisplayFinalScore(); // Show final score when max deliveries are reached
        }
    }

    public void RestartGame()
    {
        //Time.timeScale = 1f; // Resume the game
        ShowGameplayUI(); 
        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false;
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }



}