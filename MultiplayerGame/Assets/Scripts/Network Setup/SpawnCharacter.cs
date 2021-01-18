using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;
using PHashtable = ExitGames.Client.Photon.Hashtable;

[System.Serializable]
public class GOEvent : UnityEvent<GameObject> {};

public class SpawnCharacter : MonoBehaviour {
    public GameObject TeamAPrefab;
    public GameObject TeamBPrefab;
    public Transform TeamAPosition;
    public Transform TeamBPosition;
    public GOEvent PlayerSpawned;
    // Start is called before the first frame update
    void Start()
    {
        PHashtable myProperties = PhotonNetwork.LocalPlayer.CustomProperties;
        TEAMS team = TEAMS.TEAM_A;
        if (myProperties.ContainsKey("Team"))
            team = (TEAMS)myProperties["Team"];
        GameObject myPlayer = null;
        
        switch (team) {
            default:
            case TEAMS.TEAM_A:
                myPlayer = MasterManager.NetworkInstantiate(TeamAPrefab, TeamAPosition.position, Quaternion.identity);
                break;
            case TEAMS.TEAM_B:
                myPlayer = MasterManager.NetworkInstantiate(TeamBPrefab, TeamBPosition.position, Quaternion.identity);
                break;
        }

        //myPlayer.tag = "My Player";
        PlayerSpawned.Invoke(myPlayer);
    }
}
