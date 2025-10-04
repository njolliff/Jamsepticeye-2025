using System;
using UnityEngine;

public static class EventManager
{
    public static Action<Transform> onPlayerSpawned;
    public static void PlayerSpawned(Transform playerTransform) => onPlayerSpawned.Invoke(playerTransform);

    public static Action onPlayerDied;
    public static void PlayerDied() => onPlayerDied?.Invoke();

    public static Action<PlayerCorpse> onCorpseSpawned;
    public static void CorpseSpawned(PlayerCorpse corpse) => onCorpseSpawned?.Invoke(corpse);
}