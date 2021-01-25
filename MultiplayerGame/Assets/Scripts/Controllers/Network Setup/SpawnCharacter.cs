using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using PHashtable = ExitGames.Client.Photon.Hashtable;

[System.Serializable]
public class PCEvent : UnityEvent<PlayerController> {};

public class SpawnCharacter : MonoBehaviour 
{
    public GameObject TeamAPrefab;
    public GameObject TeamBPrefab;
    [LayerSelector]
    public int TeamALayer;
    [LayerSelector]
    public int TeamBLayer; 
    public Transform TeamAPosition;
    public Transform TeamBPosition;
    public PCEvent PlayerSpawned;
    // Start is called before the first frame update
    void Start()
    {
        PHashtable myProperties = PhotonNetwork.LocalPlayer.CustomProperties;
        TEAMS team = TEAMS.TEAM_A;
        if (myProperties.ContainsKey("Team"))
            team = (TEAMS)myProperties["Team"];

        PhotonNetwork.LocalPlayer.SetScore(0);

        GameObject myPlayer = null;
        
        object[] myData = new object[1];
        switch (team) {
            default:
            case TEAMS.TEAM_A:
                myData[0] = TeamALayer;
                myPlayer = MasterManager.NetworkInstantiate(TeamAPrefab, TeamAPosition.position, Quaternion.identity, myData);
                break;
            case TEAMS.TEAM_B:
                myData[0] = TeamBLayer;
                myPlayer = MasterManager.NetworkInstantiate(TeamBPrefab, TeamBPosition.position, Quaternion.identity, myData);
                break;
        }

        PlayerSpawned.Invoke(myPlayer.GetComponent<PlayerController>());
    }
}
