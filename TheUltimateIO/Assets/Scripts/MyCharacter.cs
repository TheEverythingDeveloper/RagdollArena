using Photon.Pun;
using UnityEngine;

public class MyCharacter : MonoBehaviourPun
{
    private PhotonView _view;
    private Rigidbody _rb;
    private Color _color;
    public float speed = 2f;
    public float jumpSpeed = 100f;

    private void Awake()
    {
        Debug.Log("awakee");
        _view = GetComponent<PhotonView>();
        _rb = GetComponent<Rigidbody>();
        if (!_view.IsMine)
        {
            Debug.Log("No tengo autoridad");
            return;
        }
        _color = GetComponent<Renderer>().material.color = new Color(
            Random.Range(0f, 1f),
            Random.Range(0f, 1f),
            Random.Range(0f, 1f));
        _view.RPC("RPCUpdateColor", RpcTarget.AllBuffered, _color.r, _color.g, _color.b);
    }

    private void Update()
    {
        if (!_view.IsMine) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _rb.AddForce(Vector3.up * jumpSpeed * Time.deltaTime, ForceMode.Impulse);
        }
    }

    private void FixedUpdate()
    {
        var horAxis = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        var verAxis = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        _rb.velocity = new Vector3(horAxis, _rb.velocity.y, verAxis);
    }

    [PunRPC]
    public void RPCUpdateColor(float r, float g, float b)
    {
        GetComponent<Renderer>().material.color = new Color(r, g, b, 1f);
    }
}

