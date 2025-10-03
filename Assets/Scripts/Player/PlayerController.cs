using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Variables
    public static PlayerController Instance { get; private set; }
    public bool isAlive = true;
    #endregion

    #region Initialization / Destruction
    void Awake()
    {
        // Set as singleton instance or destroy game object if instance already exists
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    void OnDestroy()
    {
        // If this was the singleton instance, set Instance to null
        if (Instance == this) Instance = null;
    }
    #endregion
}