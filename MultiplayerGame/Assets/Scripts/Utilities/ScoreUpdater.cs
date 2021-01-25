
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class ScoreUpdater : MonoBehaviour
{

    public Text TeamAScore;
    public Text TeamBScore;
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
                    scoreA += (int)player.CustomProperties["Score"];
                    break;

                case TEAMS.TEAM_B:
                    scoreB += (int)player.CustomProperties["Score"];
                    break;
            }
        } 

        TeamAScore.text = scoreA.ToString("0000");
        TeamBScore.text = scoreB.ToString("0000");
    }

    public void SetToUpdate(PlayerController player)
    {
        player.ScoreUpdated.AddListener(UpdateScores);
    }
}
