using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Variables
    public static GameManager Instance { get; private set; }
    [SerializeField] private GameObject playerPrefab;
    public Vector2 playerSpawnPoint = new();
    public int numDeaths = 0;

    public float respawnDelay = 1f;
    private float _respawnTimer = 0f;
    private bool _doRespawn = false;
    #endregion

    #region Initialization / Destruction
    void Awake()
    {
        // Set as singleton instance or destroy if instance already exists
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    void OnEnable()
    {
        // Subscribe to events
        EventManager.onPlayerDied += SpawnPlayer;
    }
    void OnDisable()
    {
        // Unsubscribe from events
        EventManager.onPlayerDied -= SpawnPlayer;
    }
    void OnDestroy()
    {
        // If this was the singleton instance, set instance to null
        if (Instance == this) Instance = null;
    }
    #endregion

    #region Update
    void Update()
    {
        HandleRespawnTimer();
    }
    #endregion

    #region Timers
    private void HandleRespawnTimer()
    {
        if (_doRespawn)
        {
            _respawnTimer += Time.deltaTime;
            if (_respawnTimer >= respawnDelay)
            {
                _doRespawn = false; // Disable respawn
                Instantiate(playerPrefab, playerSpawnPoint, Quaternion.identity); // Spawn player
                _respawnTimer = 0f; // Reset timer
            }
        }
    }
    #endregion

    #region Private Methods
    private void SpawnPlayer()
    {
        // Set respawn bool to true, spawn player after delay in Update using deltaTime
        _doRespawn = true;
    }
    #endregion

    #region Public Methods
    [ContextMenu("Kill Player")]
    public void KillPlayer()
    {
        if (PlayerController.Instance != null) PlayerController.Instance.Die();
    }
    #endregion
}