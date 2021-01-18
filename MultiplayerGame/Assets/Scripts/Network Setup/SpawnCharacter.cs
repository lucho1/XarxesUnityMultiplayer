using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;
using PHashtable = ExitGames.Client.Photon.Hashtable;

[System.Serializable]
public class GOEvent : UnityEvent<GameObject> {};

public class SpawnCharacter : MonoBehaviour {
    public GameObject TeamAPrefab;
    public GameObject TeamBPrefab;
    public Transform StartPosition;
    public GOEvent PlayerSpawned;
    // Start is called before the first frame update
    void Start()
    {
        PHashtable myProperties = PhotonNetwork.LocalPlayer.CustomProperties;
        GameObject myPlayer = MasterManager.NetworkInstantiate(TeamAPrefab, StartPosition.position, Quaternion.identity);
        //myPlayer.tag = "My Player";
        PlayerSpawned.Invoke(myPlayer);
    }
}
