using Photon.Pun;
using System.Linq;
using UnityEngine;

public class MyCharacter : MonoBehaviourPun
{
    private PhotonView _view;
    public Rigidbody pelvisRb;
    private Color _color;
    private Renderer[] _allMyRenderers;
    public float speed = 60f;
    public float jumpSpeed = 200f;
    public float rotationSpeed = 2f;
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
    private Vector3 _forwardTarget;
    private Quaternion _lookRotation;
    private Vector3 _direction;
    private Quaternion _initialRot;

    private void Awake()
    {
        _view = GetComponent<PhotonView>();
        _allMyRenderers = GetComponentsInChildren<Renderer>();
        if (!_view.IsMine)
            return;

        _initialRot = pelvisRb.transform.localRotation;
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
        if (!_view.IsMine && pelvisRb != null) return;
        if (Input.GetKeyDown(KeyCode.Space) && !_inAir)
        {
            _inAir = true;
            pelvisRb.AddForce(Vector3.up * jumpSpeed * Time.deltaTime, ForceMode.Impulse);
        }

        Debug.DrawLine(transform.position, 
            transform.position + Vector3.down * inAirDistance,
            _inAir ? Color.red : Color.green);
        _inAir = !Physics.Raycast(pelvisRb.transform.position, Vector3.down, inAirDistance, 1 << 9);

        _sqrMagnitudeInTime = Mathf.Lerp(_sqrMagnitudeInTime, pelvisRb.velocity.sqrMagnitude,
            sqrMagnitudeInTimeSpeed * Time.deltaTime);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1 << 9))
        {
            _direction = (hit.point - pelvisRb.transform.position).normalized;

            _lookRotation = Quaternion.LookRotation(_direction);
            _lookRotation *= _initialRot;

            pelvisRb.transform.localRotation = Quaternion.Slerp(pelvisRb.transform.localRotation, _lookRotation, Time.deltaTime * rotationSpeed);
        }
    }

    private void FixedUpdate()
    {
        if (!_view.IsMine) return;

        //Movement
        var horAxis = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        var verAxis = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        pelvisRb.velocity = new Vector3(horAxis, pelvisRb.velocity.y, verAxis);

        #region Camera
        //FoV Camera
        Camera.main.fieldOfView = Mathf.Lerp(minFOV, maxFOV, pelvisRb.velocity.sqrMagnitude * ratioMultiplierFoV);

        //Camera offset
        Vector3 offsetPosition = pelvisRb.transform.position + cameraOffset;
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, 
            offsetPosition, cameraSpeed * Time.deltaTime);
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

