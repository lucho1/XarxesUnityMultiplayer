using UnityEngine;
using UnityEngine.UI;

public class RespawnUI : MonoBehaviour
{
    public Text SecondsText;
    private BackTimer m_DeathTimer;

    void Update() {
        SecondsText.text = m_DeathTimer.GetTimeLeftInSeconds().ToString();;
    }

    public void SetPlayer(PlayerController player)
    {
        m_DeathTimer = player.gameObject.GetComponent<BackTimer>();
        player.PlayerRespawn.AddListener(Respawning);
        player.PlayerDead.AddListener(PlayerDeath);
    }

    public void Respawning() {
        gameObject.SetActive(false);
    }

    public void PlayerDeath() {
        gameObject.SetActive(true);
        SecondsText.text = m_DeathTimer.GetTimeLeftInSeconds().ToString();
    }
}
