using Photon.Pun;
using UnityEngine;

public class MyCharacter : MonoBehaviourPun
{
    private PhotonView _view;
    private Rigidbody _rb;
    private Color _color;
    public float speed = 60f;
    public float jumpSpeed = 200f;
    public float cameraSpeed = 0.6f;
    private bool _inAir = false;
    [Tooltip("Distancia que vamos a necesitar estar del piso para poder saltar.")]
    public float inAirDistance = 0.6f;
    public float minFOV;
    public float maxFOV;
    [Tooltip("Offset de la camara con respecto al character")]
    public Vector3 cameraOffset = new Vector3(-0.01f, 5.9f, -4f);
    [Tooltip("Mientras mas bajo, mas va a quedar en el MinFoV. Caso contrario, del MaxFoV.")]
    public float ratioMultiplierFoV;
    private float _sqrMagnitudeInTime = 0f;
    public float sqrMagnitudeInTimeSpeed;

    private void Awake()
    {
        Debug.Log("awakee");
        _view = GetComponent<PhotonView>();
        _rb = GetComponentInChildren<Rigidbody>();
        if (!_view.IsMine)
        {
            Debug.Log("No tengo autoridad");
            return;
        }
        //TODO: Cambiar el color de todo el robot
        _color = GetComponent<Renderer>().material.color = new Color(
            Random.Range(0f, 1f),
            Random.Range(0f, 1f),
            Random.Range(0f, 1f));
        _view.RPC("RPCUpdateColor", RpcTarget.AllBuffered, _color.r, _color.g, _color.b);
    }

    private void Update()
    {
        if (!_view.IsMine) return;
        if (Input.GetKeyDown(KeyCode.Space) && !_inAir)
        {
            _inAir = true;
            _rb.AddForce(Vector3.up * jumpSpeed * Time.deltaTime, ForceMode.Impulse);
        }

        Debug.DrawLine(transform.position, transform.position + Vector3.down * inAirDistance, _inAir? Color.red : Color.green);
        _inAir = !Physics.Raycast(transform.position, Vector3.down, inAirDistance, 1 << 9);

        _sqrMagnitudeInTime = Mathf.Lerp(_sqrMagnitudeInTime, _rb.velocity.sqrMagnitude, sqrMagnitudeInTimeSpeed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (!_view.IsMine) return;

        //Movement
        var horAxis = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        var verAxis = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        _rb.velocity = new Vector3(horAxis, _rb.velocity.y, verAxis);


        #region Camera
        //FoV Camera
        Camera.main.fieldOfView = Mathf.Lerp(minFOV, maxFOV, _rb.velocity.sqrMagnitude * ratioMultiplierFoV);

        //Camera offset
        Vector3 offsetPosition = _rb.transform.position + cameraOffset;
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, offsetPosition, cameraSpeed * Time.deltaTime);
        #endregion
    }

    [PunRPC]
    public void RPCUpdateColor(float r, float g, float b)
    {
        GetComponent<Renderer>().material.color = new Color(r, g, b, 1f);
    }
}

