using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkCharacter : MonoBehaviourPunCallbacks, IPunObservable
{

    private Vector3 position;
    private Quaternion rotation;
    private float smoothing = 10.0f;

    [SerializeField]
    private GameObject cameraObject;
    [SerializeField]
    private GameObject playerObject;
    void Awake()
    {
        Debug.Log("Awake!");
        if (photonView.IsMine)
        {
            Debug.Log("It is mine");
            cameraObject.SetActive(true);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
        }
        else
        {
            position = transform.position;
            rotation = transform.rotation;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine)
        {
            transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * smoothing);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * smoothing);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            position = (Vector3)stream.ReceiveNext();
            rotation = (Quaternion)stream.ReceiveNext();
        }
    }
}