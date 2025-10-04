using UnityEngine;

public class PlayerCorpse : MonoBehaviour
{
    #region Variables
    public static int maxCorpses = 3;
    public int corpseCounter = 1;
    #endregion

    #region Initialization / Destruction
    void Awake()
    {
        EventManager.CorpseSpawned(this); // Call the corpse spawned event when instantiated
    }
    void OnEnable()
    {
        EventManager.onCorpseSpawned += CheckCorpseDespawn;
    }
    void OnDisable()
    {
        EventManager.onCorpseSpawned -= CheckCorpseDespawn;
    }
    #endregion

    #region Check Despawn
    private void CheckCorpseDespawn(PlayerCorpse corpse)
    {
        // Because the event is called before it is subscribed to, the corpse shouldn't be able to count itself, but double check just to be safe
        if (corpse != this)
        {
            corpseCounter++;
            if (corpseCounter > maxCorpses) Destroy(gameObject);
        }
    }
    #endregion
}