using UnityEngine;
using Photon.Pun;

public class SpawnCharacter : MonoBehaviour {
    public string Address;
    public Transform StartPosition;
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.Instantiate(Address, StartPosition.position, Quaternion.identity);
    }
}
