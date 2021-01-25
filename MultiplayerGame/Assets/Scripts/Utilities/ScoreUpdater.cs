
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

public class ScoreUpdater : MonoBehaviour
{

    public Text TeamAScore;
    public Text TeamBScore;
    void Start() {

    }
    // Update is called once per frame
    public void UpdateScores()
    {
        int scoreA = 0;
        int scoreB = 0;
        foreach (Player player in PhotonNetwork.PlayerList) {
            TEAMS team = TEAMS.NONE;
            
            team = (TEAMS)player.CustomProperties["Team"];

            switch (team) {
                case TEAMS.TEAM_A:
                    scoreA += player.GetScore();
                    break;

                case TEAMS.TEAM_B:
                    scoreB += player.GetScore();
                    break;
            }
        } 

        TeamAScore.text = scoreA.ToString("0000");
        TeamBScore.text = scoreB.ToString("0000");
        InvokeRepeating("UpdateScores", 1, 0.5f); 
    }

    public void SetToUpdate(PlayerController player)
    {
        player.ScoreUpdated.AddListener(UpdateScores);
    }
}
