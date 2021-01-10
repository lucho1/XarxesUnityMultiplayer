using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PlayerListController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Transform ListContent;

    [SerializeField]
    private GameObject ListElement;
    private List<GameObject> m_ExistingPlayersList = new List<GameObject>();

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        GameObject list_element = Instantiate(ListElement, ListContent);
        if (list_element)
        {
            list_element.GetComponentInChildren<Text>().text = newPlayer.NickName;
            m_ExistingPlayersList.Add(list_element);
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        foreach (GameObject obj in m_ExistingPlayersList)
        {
            if (obj.GetComponentInChildren<Text>().text == otherPlayer.NickName)
            {
                m_ExistingPlayersList.Remove(obj);
                Destroy(obj);
                break;
            }
        }
    }
}
