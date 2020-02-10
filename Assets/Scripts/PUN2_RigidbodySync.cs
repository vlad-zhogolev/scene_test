using UnityEngine;
using Photon.Pun;

public class PUN2_RigidbodySync : MonoBehaviourPun, IPunObservable
{

    Rigidbody r;

    Vector3 latestPos;
    Quaternion latestRot;
    Vector3 velocity;
    Vector3 angularVelocity;

    bool photonViewOwnerChanged = false;
    bool valuesReceived = false;

    // Start is called before the first frame update
    void Start()
    {
        r = GetComponent<Rigidbody>();
        //Debug.LogFormat("PUN2_RigidbodySync: received rigidbody = {0}", r != null);
        //Debug.LogFormat("PUN2_RigidbodySync: start IsMine = {0}", photonView.IsMine);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //Debug.Log("PUN2_RigidbodySync: OnPhotonSerializeView");
        if (stream.IsWriting)
        {
            //Debug.LogFormat("PUN2_RigidbodySync: Writing position for {0}", gameObject.name);
            //We own this player: send the others our data
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(r.velocity);
            stream.SendNext(r.angularVelocity);
        }
        else
        {
            //Debug.LogFormat("PUN2_RigidbodySync: Reading position for {0}", gameObject.name);
            //Network player, receive data
            latestPos = (Vector3)stream.ReceiveNext();
            latestRot = (Quaternion)stream.ReceiveNext();
            velocity = (Vector3)stream.ReceiveNext();
            angularVelocity = (Vector3)stream.ReceiveNext();

            valuesReceived = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (photonViewOwnerChanged)
        {
            //Debug.LogFormat("PUN2_RigidbodySync: photonView of object {0} changed, IsMine = {1}", gameObject.name, photonView.IsMine);
            photonViewOwnerChanged = false;
        }
        if (!photonView.IsMine && valuesReceived)
        {
            //Debug.LogFormat("PUN2_RigidbodySync: Update rigidbody: {0}", gameObject.name);
            //Update Object position and Rigidbody parameters
            transform.position = Vector3.Lerp(transform.position, latestPos, Time.deltaTime * 5);
            transform.rotation = Quaternion.Lerp(transform.rotation, latestRot, Time.deltaTime * 5);
            r.velocity = velocity;
            r.angularVelocity = angularVelocity;
        }
    }

    void OnCollisionEnter(Collision contact)
    {
        //Debug.LogFormat("PUN2_RigidbodySync: Collision detected, this: {0}, other: {1}, photonView.IsMine: {2}", gameObject.name, contact.gameObject.name, photonView.IsMine);
        if (!photonView.IsMine)
        {
            Transform collisionObjectRoot = contact.transform.root;
            //Debug.LogFormat("PUN2_RigidbodySync: Collision collisionObjectRoot: {0}, it's tag: {1}", collisionObjectRoot.gameObject.name, collisionObjectRoot.tag);
            if (collisionObjectRoot.CompareTag(NetworkingPlayer.PLAYER_TAG))
            {
                //Transfer PhotonView of Rigidbody to our local player
                //Debug.LogFormat("PUN2_RigidbodySync: Transferring ownership to: {0}", PhotonNetwork.LocalPlayer);
                photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
                photonViewOwnerChanged = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.LogFormat("PUN2_RigidbodySync: Trigger enter detected, this: {0}, other: {1}, photonView.IsMine: {2}", gameObject.name, other.gameObject.name, photonView.IsMine);
        if (!photonView.IsMine)
        {
            Transform collisionObjectRoot = other.transform.root;
            //Debug.LogFormat("PUN2_RigidbodySync: Trigger collisionObjectRoot: {0}, it's tag: {1}", collisionObjectRoot.gameObject.name, collisionObjectRoot.tag);
            if (collisionObjectRoot.CompareTag(NetworkingPlayer.PLAYER_TAG))
            {
                //Transfer PhotonView of Rigidbody to our local player
                //Debug.LogFormat("PUN2_RigidbodySync: Transferring ownership to: {0}", PhotonNetwork.LocalPlayer);
                photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
                photonViewOwnerChanged = true;
            }
        }
    }
}