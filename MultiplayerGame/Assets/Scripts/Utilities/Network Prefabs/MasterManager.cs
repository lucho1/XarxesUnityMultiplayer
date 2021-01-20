using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Photon.Pun;

[CreateAssetMenu(menuName = "Singletons/MasterManager")]
public class MasterManager : ScriptableObject
{
    private static MasterManager m_instance;
    private static bool m_isInstanced = false;
    public static MasterManager Instance {
        get {
            if (m_isInstanced) return m_instance;
            MasterManager[] assets = Resources.LoadAll<MasterManager>("");
            if (assets.Length > 1)
                Debug.LogError("Found multiple MasterManager on the Resources folder, there should only be one!");
            if (assets.Length == 0)
            {
                m_instance = CreateInstance<MasterManager>();
                Debug.LogError("Could not find a MasterManager on Resources folder, one was creatd at runtime but it will not persist!");
            }
            else
                m_instance = assets[0];
            
            m_isInstanced = true;
            return m_instance;

            }
    }

    [SerializeField]
    private List<NetworkedPrefab> m_networkedPrefabs = new List<NetworkedPrefab>();

    public static GameObject NetworkInstantiate(GameObject prefab, Vector3 position, Quaternion rotation, object[] data = null) {
        foreach (NetworkedPrefab netPrefab in Instance.m_networkedPrefabs) {
            if (netPrefab.Prefab == prefab)
            {
                GameObject res = PhotonNetwork.Instantiate(netPrefab.Path, position, rotation, 0, data);
                return res;
            }
        }
        return null;
    }

    #if UNITY_EDITOR

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    [MenuItem("Networking/Network Prefabs/Populate")]
    private static void PopulateNetworkPrefabs() {
        Debug.Log("Populating Network Prefabs");
        Instance.m_networkedPrefabs.Clear();
        GameObject[] prefabs = Resources.LoadAll<GameObject>("");
        foreach (GameObject prefab in prefabs) {
            if (prefab.GetComponent<PhotonView>() != null) {
                string path = AssetDatabase.GetAssetPath(prefab);
                Instance.m_networkedPrefabs.Add(new NetworkedPrefab(prefab, path));
            }
                
        }
    }

    #endif
}

