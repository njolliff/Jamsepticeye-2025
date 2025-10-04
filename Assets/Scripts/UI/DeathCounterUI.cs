using TMPro;
using UnityEngine;

public class DeathCounterUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI counterText; // TMP component reference

    // Event Subscription / Unsubscription
    void OnEnable()
    {
        EventManager.onPlayerDied += UpdateCounterText;
    }
    void OnDisable()
    {
        EventManager.onPlayerDied -= UpdateCounterText;
    }

    // Text update method
    public void UpdateCounterText()
    {
        counterText.text = $": {GameManager.Instance.numDeaths}";
    }
}