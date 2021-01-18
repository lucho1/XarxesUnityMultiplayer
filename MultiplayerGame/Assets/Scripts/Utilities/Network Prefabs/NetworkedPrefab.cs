using UnityEngine;

[System.Serializable]
public class NetworkedPrefab {

    public GameObject Prefab;
    public string Path;

    public NetworkedPrefab (GameObject prefab, string path) {
        Prefab = prefab;
        Path = FormatPath(path);
    }

    private string FormatPath(string path) {
        int extensionLength = System.IO.Path.GetExtension(path).Length;
        int resourcesIndex = path.ToLower().IndexOf("resources");

        if (resourcesIndex == -1)
            return string.Empty;
        else {
            int startIndex = path.ToLower().IndexOf('/', resourcesIndex) + 1;
            return path.Substring(startIndex, path.Length - (startIndex + extensionLength));
        }
    }

}
