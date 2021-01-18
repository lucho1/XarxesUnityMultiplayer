using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Photon.Pun;

[CreateAssetMenu(menuName = "Singletons/MasterManager")]
public class MasterManager : ScriptableSingleton<MasterManager>
{
    [SerializeField]
    private List<NetworkedPrefab> m_networkedPrefabs = new List<NetworkedPrefab>();

    public static GameObject NetworkInstantiate(GameObject prefab, Vector3 position, Quaternion rotation) {
        foreach (NetworkedPrefab netPrefab in instance.m_networkedPrefabs) {
            if (netPrefab.Prefab == prefab)
            {
                GameObject res = PhotonNetwork.Instantiate(netPrefab.Path, position, rotation);
                return res;
            }
        }
        return null;
    }

    #if UNITY_EDITOR

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    [MenuItem("Networking/NetworkPrefabs/PopulateNetworkPrefabs")]
    private static void PopulateNetworkPrefabs() {
        Debug.Log("Populating Network Prefabs");
        instance.m_networkedPrefabs.Clear();
        GameObject[] prefabs = Resources.LoadAll<GameObject>("");
        foreach (GameObject prefab in prefabs) {
            if (prefab.GetComponent<PhotonView>() != null) {
                string path = AssetDatabase.GetAssetPath(prefab);
                instance.m_networkedPrefabs.Add(new NetworkedPrefab(prefab, path));
            }
                
        }
    }

    #endif
}

