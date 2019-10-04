using Photon.Pun;
using System.Linq;
using UnityEngine;

public class MyCharacter : MonoBehaviourPun
{
    private PhotonView _view;
    private Rigidbody _rb;
    private Color _color;
    private Renderer[] _allMyRenderers;
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
        _view = GetComponent<PhotonView>();
        _rb = GetComponentInChildren<Rigidbody>();
        _allMyRenderers = GetComponentsInChildren<Renderer>();
        if (!_view.IsMine)
            return;

        var colorA = _allMyRenderers[0].material.GetColor("_ColorA");
        var colorB = _allMyRenderers[0].material.GetColor("_ColorB");
        var colorC = _allMyRenderers[0].material.GetColor("_ColorC");
        _view.RPC("RPCUpdateColor", RpcTarget.AllBuffered,
            new float[] { colorA.r, colorA.g, colorA.b },
            new float[] { colorB.r, colorB.g, colorB.b },
            new float[] { colorC.r, colorC.g, colorC.b });
    }

    private void Update()
    {
        if (!_view.IsMine) return;
        if (Input.GetKeyDown(KeyCode.Space) && !_inAir)
        {
            _inAir = true;
            _rb.AddForce(Vector3.up * jumpSpeed * Time.deltaTime, ForceMode.Impulse);
        }

        Debug.DrawLine(transform.position, transform.position + Vector3.down * inAirDistance, _inAir ? Color.red : Color.green);
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
    public void RPCUpdateColor(float[] colorA, float[] colorB, float[] colorC)
    {
        _allMyRenderers.Select(x =>
        {
            x.material.SetColor("_ColorA", new Color(colorA[0],colorA[1],colorA[2]));
            x.material.SetColor("_ColorB", new Color(colorB[0], colorB[1], colorB[2]));
            x.material.SetColor("_ColorC", new Color(colorC[0], colorC[1], colorC[2]));
            return x;
        }).ToList();
    }
}

