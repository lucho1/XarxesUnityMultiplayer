using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using PHashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerController : MonoBehaviour, IPunInstantiateMagicCallback
{
    public float PlayerSpeed = 2.0f;
    [Range(0.0f, 1.0f)] public float PlayerAcceleration = 0.10f;
    public bool NetworkMode = true;
    [TagSelector]
    public string BulletTag;
    public float RespawnTime = 5.0f;
    public int DeathScore = 100;  
    public GameObject ScorePopupPrefab;
    public Transform ScorePopupPosition;

    private Vector3 m_OriginalPosition;
    private BackTimer m_RespawnTimer;
    private PhotonView m_PhotonView;
    private CharacterController m_CharacterController;
    private Animator m_Animator;

    private Vector3 m_CurrentSpeed = Vector3.zero;
    private Vector3 m_CurrentRotation = Vector3.zero;
    private Rigidbody m_RigidBody;

    private Vector3 m_LastMousePos;

    private bool m_IsDead = false;
    private List<Renderer> m_ChildRenderers;

    public UnityEvent PlayerDead;
    public UnityEvent PlayerRespawn;
    public UnityEvent ScoreUpdated;

    private bool m_isKinematic;
    private bool m_detectCollisions;

    // Start is called before the first frame update
    void Awake()
    {
        m_ChildRenderers = new List<Renderer>(gameObject.GetComponentsInChildren<Renderer>());
        for (int i = 0; i < m_ChildRenderers.Count;)
        {
            if (!m_ChildRenderers[i].enabled)
                m_ChildRenderers.RemoveAt(i);
            else
                ++i;
        }
        m_RespawnTimer = gameObject.GetComponent<BackTimer>();
        m_RespawnTimer.StartTime = RespawnTime;

        m_PhotonView = GetComponent<PhotonView>();
        if (NetworkMode && m_PhotonView && !m_PhotonView.IsMine)
            this.enabled = false;

        m_CharacterController = GetComponent<CharacterController>();
        m_Animator = gameObject.GetComponentInChildren<Animator>();
        m_RigidBody = gameObject.GetComponent<Rigidbody>();
        m_OriginalPosition = transform.position;

        m_isKinematic = m_RigidBody.isKinematic;
        m_detectCollisions = m_RigidBody.detectCollisions;


        m_LastMousePos = Input.mousePosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_IsDead)
        {
            if (m_PhotonView.IsMine)
                RespawnUpdate();
            return;
        }
        if(!m_RigidBody)
            m_RigidBody = gameObject.GetComponent<Rigidbody>();

        // If player has a Rigid Body, let's move!
        if (m_RigidBody)
        {
            // --- ROTATION ---
            // Decide to rotate with mouse (if moved) or with gamepad
            Quaternion rotation = Quaternion.identity;
            if (Input.mousePosition != m_LastMousePos)
            {
                m_LastMousePos = Input.mousePosition;

                Vector3 dir = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                rotation = Quaternion.AngleAxis(-angle, Vector3.up);
            }
            else
            {
                Vector3 desired_lookat_speed = new Vector3(-Input.GetAxis("RightVertical"), 0, Input.GetAxis("RightHorizontal"));
                m_CurrentRotation += (desired_lookat_speed - m_CurrentRotation) * Time.deltaTime * 10.0f;

                if (m_CurrentRotation != Vector3.zero)
                    rotation = Quaternion.LookRotation(m_CurrentRotation.normalized, Vector3.up);
            }

            // Apply Rotation to Rigid Body
            if (rotation != Quaternion.identity)
                m_RigidBody.MoveRotation(rotation);


            // --- MOVEMENT ---
            // Compute speed and apply acceleration
            Vector3 desired_speed = PlayerSpeed * new Vector3(-Input.GetAxis("Vertical"), 0, Input.GetAxis("Horizontal"));
            m_CurrentSpeed += PlayerAcceleration * (desired_speed - m_CurrentSpeed);

            // Apply speed to position
            transform.position += m_CurrentSpeed * Time.deltaTime;


            // --- ANIMATION ---
            if (desired_speed != Vector3.zero)
                m_Animator.SetBool("Running", true);
            else
                m_Animator.SetBool("Running", false);
        }
        else
            Debug.LogError("Player Has Not RigidBody!!!!!");
    }


    private void OnTriggerEnter(Collider other) 
    {
        //This should really not enter here on remote, but just in case
        if (!m_PhotonView.IsMine || !other.gameObject.CompareTag(BulletTag))
            return;
        
        PhotonView o_pv = other.gameObject.GetPhotonView();
        ((PlayerController)o_pv.Owner.TagObject).AddScore(DeathScore);
        
        m_PhotonView.RPC("Death", RpcTarget.All);
    }

    private void OnCollisionEnter(Collision other) 
    {
        //This should really not enter here on remote, but just in case
        if (!m_PhotonView.IsMine || !other.gameObject.CompareTag(BulletTag))
            return;
        
        PhotonView o_pv = other.gameObject.GetPhotonView();
        ((PlayerController)o_pv.Owner.TagObject).AddScore(DeathScore);
        
        m_PhotonView.RPC("Death", RpcTarget.All);
    }

    [PunRPC]
    private void Death() 
    {
        m_IsDead = true;
        foreach (Renderer childRenderer in m_ChildRenderers)
            childRenderer.enabled = false;

        m_RigidBody.isKinematic = false;
        m_RigidBody.detectCollisions = false;
        
        if (m_PhotonView.IsMine) 
        {
            m_RespawnTimer.Begin();
            PlayerDead.Invoke();
        }

    }

    [PunRPC]
    private void Respawn() 
    {
        m_IsDead = false;
        foreach (Renderer childRenderer in m_ChildRenderers)
            childRenderer.enabled = true;

        m_RigidBody.isKinematic = m_isKinematic;
        m_RigidBody.detectCollisions = m_detectCollisions;

        if (m_PhotonView.IsMine) 
        {
            transform.position = m_OriginalPosition;
            PlayerRespawn.Invoke();
        }
    }

    public void AddScore(int scoreAmount) {
        m_PhotonView.RPC("SpawnPopup", RpcTarget.All, scoreAmount);
        m_PhotonView.RPC("PunAddScore", RpcTarget.MasterClient, scoreAmount);
    }

    [PunRPC]
    private void SpawnPopup(int scoreAmount) {
        ScoreUpdated.Invoke();
        if (!m_PhotonView.IsMine)
            return;

        PopupScore.CreatePopup(ScorePopupPrefab, ScorePopupPosition.position, scoreAmount);
    }

    [PunRPC]
    private void PunAddScore(int scoreAmount) 
    {
        m_PhotonView.Owner.AddScore(scoreAmount);
    }

    private void RespawnUpdate()
    {
        if (m_RespawnTimer.GetTimeLeftInSeconds() <= 0)
            m_PhotonView.RPC("Respawn", RpcTarget.All);
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] instantiatonData = info.photonView.InstantiationData;
        this.gameObject.layer = (int) instantiatonData[0];
        m_PhotonView.Owner.TagObject = this;
    }
}