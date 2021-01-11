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

    public void Update()
    {
        if (PhotonNetwork.InRoom)
        {
            foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
            {
                bool found_player = false;

                foreach (GameObject obj in m_ExistingPlayersList)
                {
                    if (obj.GetComponentInChildren<Text>().text == player.Value.NickName)
                    {
                        found_player = true;
                        break;
                    }
                }

                if (!found_player)
                {
                    GameObject list_element = Instantiate(ListElement, ListContent);
                    if (list_element)
                    {
                        list_element.GetComponentInChildren<Text>().text = player.Value.NickName;
                        m_ExistingPlayersList.Add(list_element);
                    }
                }
            }
        }
    }

    public override void OnEnable()
    {
        SetPlayersInRoom();
    }

    public void SetPlayersInRoom()
    {
        if (!PhotonNetwork.IsConnected)
            return;
        if (PhotonNetwork.CurrentRoom == null || PhotonNetwork.CurrentRoom.Players == null)
            return;

        if (PhotonNetwork.InRoom)
        {
            foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
            {
                GameObject list_element = Instantiate(ListElement, ListContent);
                if (list_element)
                {
                    list_element.GetComponentInChildren<Text>().text = player.Value.NickName;
                    m_ExistingPlayersList.Add(list_element);
                }
            }
        }
    }

    

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        int index = m_ExistingPlayersList.FindIndex(x => x.GetComponentInChildren<Text>().text == newPlayer.NickName);

        if (index == -1)
        {
            GameObject list_element = Instantiate(ListElement, ListContent);
            if (list_element)
            {
                list_element.GetComponentInChildren<Text>().text = newPlayer.NickName;
                m_ExistingPlayersList.Add(list_element);
            }
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

    public override void OnDisable()
    {
        foreach (GameObject list_element in m_ExistingPlayersList)
        {
            m_ExistingPlayersList.Remove(list_element);
            Destroy(list_element);
        }

        m_ExistingPlayersList.Clear();
    }
}
