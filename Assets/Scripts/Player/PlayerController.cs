using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Variables
    public static PlayerController Instance { get; private set; }

    [Header("References")]
    [SerializeField] private PlayerMovement _playerMovementScript;
    [SerializeField] private GameObject _playerCorpse;

    [Header("Variables")]
    public bool isAlive = true;
    #endregion

    #region Initialization / Destruction
    void Awake()
    {
        // Set as singleton instance or destroy game object if instance already exists
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    void Start()
    {
        // Trigger player spawned event
        EventManager.PlayerSpawned(transform);
    }
    void OnDestroy()
    {
        // Set instance to null if this was instance
        if (Instance == this) Instance = null;
    }
    #endregion

    [ContextMenu("Die")]
    public void Die()
    {
        if (_playerMovementScript != null) _playerMovementScript.Freeze(); // Freeze player movement

        isAlive = false; // Set player as dead

        if (GameManager.Instance != null) GameManager.Instance.numDeaths++; // Increment deaths

        if (_playerCorpse != null) Instantiate(_playerCorpse, transform.position, Quaternion.identity); // Instantiate a new corpse

        EventManager.PlayerDied(); // Invoke the player death event

        Destroy(gameObject); // Destroy player
    }
}