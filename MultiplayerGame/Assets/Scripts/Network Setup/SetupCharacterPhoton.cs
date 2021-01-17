using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SetupCharacterPhoton : MonoBehaviour
{
    PhotonView pView;
    [SerializeField]
    public GameObject[] GameobjectsToDisable;
    // Start is called before the first frame update
    void Start()
    {
        pView = gameObject.GetComponent<PhotonView>();

        if (!pView.IsMine) {
            foreach (GameObject go in GameobjectsToDisable)
                go.SetActive(false);
        }

    }
}
