using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;

[System.Serializable]
public class GOEvent : UnityEvent<GameObject> {};

public class SpawnCharacter : MonoBehaviour {
    public string Address;
    public Transform StartPosition;
    public GOEvent PlayerSpawned;
    // Start is called before the first frame update
    void Start()
    {
        GameObject myPlayer = PhotonNetwork.Instantiate(Address, StartPosition.position, Quaternion.identity);
        myPlayer.tag = "My Player";
        PlayerSpawned.Invoke(myPlayer);
    }
}
