using UnityEngine;
using TMPro;

public class PopupScore : MonoBehaviour
{
    public float DisappearTime = 1.0f;
    public float MoveSpeedY = 20.0f;
    public float DisappearSpeed = 3.0f;

    private TextMeshPro m_TextMeshPro;
    private Color m_Color;

    void Awake()
    {
        m_TextMeshPro = gameObject.GetComponent<TextMeshPro>();
        FaceTextMeshToCamera();
    }

    void Setup(int scoreAmount) 
    {
        m_TextMeshPro.SetText(scoreAmount.ToString());
        m_Color = m_TextMeshPro.color;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Camera.main.transform.up * Time.deltaTime;
        DisappearTime -= Time.deltaTime;

        if (DisappearTime < 0) //Start fading
        {
            m_Color.a -= DisappearSpeed * Time.deltaTime;
            m_TextMeshPro.color = m_Color;
            if (m_Color.a <= 0)
                Destroy(gameObject);
        }
    }

    void FaceTextMeshToCamera(){
        Vector3 origRot = transform.eulerAngles;
        transform.LookAt(Camera.main.transform);
        origRot.y = -transform.eulerAngles.y;
        transform.eulerAngles = origRot;
    }  

    public static PopupScore CreatePopup(GameObject textPrefab, Vector3 position, int scoreAmount) {
        GameObject newInstance = MasterManager.NetworkInstantiate(textPrefab, position, Quaternion.identity);

        if (newInstance == null)
            return null;
        
        PopupScore myPopup = newInstance.GetComponent<PopupScore>();
        myPopup.Setup(scoreAmount);

        return myPopup;
    }
}
