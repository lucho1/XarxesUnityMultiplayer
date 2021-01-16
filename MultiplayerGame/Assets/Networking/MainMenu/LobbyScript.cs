using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyScript : MonoBehaviour
{
    [SerializeField]
    private ConnectionAPIScript ConnectionManager;
    
    [SerializeField]
    private InputField UsernameInput;

    [SerializeField]
    private Transform ListContainer;

    [SerializeField]
    private GameObject ListElement;
    private string m_RoomSelected = "";
    private Dictionary<string, GameObject> m_RoomList = new Dictionary<string, GameObject>();


    // Start is called before the first frame update
    private void Start()
    {
        UsernameInput.textComponent.text = ConnectionManager.GetUsername();
        UsernameInput.text = ConnectionManager.GetUsername();
    }

    public void DeselectRoom()
    {
        if (m_RoomList.ContainsKey(m_RoomSelected))
        {
            m_RoomList[m_RoomSelected].GetComponentInChildren<Button>().interactable = true;
            m_RoomSelected = "";
        }
    }

    public void SetCurrentSelectedRoom(string room_name)
    {
        if(room_name != m_RoomSelected && m_RoomList.ContainsKey(room_name))
        {
            if(m_RoomList.ContainsKey(m_RoomSelected))
                m_RoomList[m_RoomSelected].GetComponentInChildren<Button>().interactable = true;

            m_RoomList[room_name].GetComponentInChildren<Button>().interactable = false;
            m_RoomSelected = room_name;
        }
    }

    private void AddRoomToList(string room_name)
    {
        GameObject new_element = Instantiate(ListElement, ListContainer);
        new_element.GetComponent<ListButtonOnClick>().LobbyObject = gameObject;
        new_element.GetComponentInChildren<Text>().text = room_name;

        m_RoomList.Add(room_name, new_element);
    }


    // --- Connection Callbacks ---
    public void RoomListUpdate(bool removed, string room_name)
    {
        if(removed)
        {
            if (m_RoomList.ContainsKey(room_name))
            {
                Destroy(m_RoomList[room_name]);
                m_RoomList.Remove(room_name);
            }
        }
        else if (!m_RoomList.ContainsKey(room_name))
            AddRoomToList(room_name);
    }


    // --- UI Callbacks ---
    public void SetUsername(Text name)
    {
        ConnectionManager.SetUsername(name.text);
    }

    public void JoinRoomButton()
    {
        ConnectionManager.JoinRoom(m_RoomSelected);
    }

    public void HostRoomButton(Text room_name)
    {
        ConnectionManager.HostRoom(room_name.text);
    }

    public void QuickStartButton()
    {
        ConnectionManager.JoinRandomRoom();
    }
}
