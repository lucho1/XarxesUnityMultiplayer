using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;
using PHashtable = ExitGames.Client.Photon.Hashtable;

[System.Serializable]
public class GOEvent : UnityEvent<GameObject> {};

public class SpawnCharacter : MonoBehaviour {
    public GameObject TeamAPrefab;
    public GameObject TeamBPrefab;
    [LayerSelector]
    public int TeamALayer;
    [LayerSelector]
    public int TeamBLayer; 
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
                myPlayer.layer = TeamALayer;
                break;
            case TEAMS.TEAM_B:
                myPlayer = MasterManager.NetworkInstantiate(TeamBPrefab, TeamBPosition.position, Quaternion.identity);
                myPlayer.layer = TeamBLayer;
                break;
        }

        //myPlayer.tag = "My Player";
        PlayerSpawned.Invoke(myPlayer);
    }
}
