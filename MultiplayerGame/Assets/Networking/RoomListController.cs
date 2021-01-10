using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class RoomListController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Transform ListContent;

    [SerializeField]
    private GameObject ListElement;
    private List<GameObject> m_ExistingRoomsList = new List<GameObject>();

    public string CurrentSelectedRoom = "";

    public void SetSelectedRoom(Text RoomName)
    {
        CurrentSelectedRoom = RoomName.text;
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach(RoomInfo room in roomList)
        {
            if (room.RemovedFromList)
            {
                foreach(GameObject obj in m_ExistingRoomsList)
                {
                    if(obj.GetComponent<Text>().text == room.Name)
                    {
                        m_ExistingRoomsList.Remove(obj);
                        Destroy(obj);
                        break;
                    }
                }
            }
            else
            {
                GameObject list_element = Instantiate(ListElement, ListContent);
                if (list_element)
                {
                    list_element.GetComponentInChildren<Text>().text = room.Name;
                    m_ExistingRoomsList.Add(list_element);
                }
            }
        }
    }
}
