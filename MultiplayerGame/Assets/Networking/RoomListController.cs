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
    private Button m_LastSelectedButton = null;

    public void SetSelectedRoom(Text RoomName)
    {
        if (m_LastSelectedButton && m_LastSelectedButton.GetComponentInParent<Text>() == RoomName)
            return;

        if (m_LastSelectedButton)
            m_LastSelectedButton.interactable = true;

        CurrentSelectedRoom = RoomName.text;
        m_LastSelectedButton = RoomName.gameObject.GetComponentInChildren<Button>();

        foreach (GameObject list_element in m_ExistingRoomsList)
            if (list_element.GetComponentInChildren<Button>() == m_LastSelectedButton)
                list_element.GetComponentInChildren<Button>().interactable = false;
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach(RoomInfo room in roomList)
        {
            if (room.RemovedFromList)
            {
                foreach(GameObject obj in m_ExistingRoomsList)
                {
                    Text obj_text = obj.GetComponentInChildren<Text>();
                    if (obj_text.text == room.Name)
                    {
                        if (obj_text.GetComponentInChildren<Button>() == m_LastSelectedButton)
                            m_LastSelectedButton = null;

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
