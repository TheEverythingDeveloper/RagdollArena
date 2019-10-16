using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Character
{
    public class CharacterModel : MonoBehaviourPun
    {
        private List<IUpdatable> _allUpdatables = new List<IUpdatable>();
        private List<IConstructable> _allConstructables = new List<IConstructable>();
        private CharacterMovement _movementController;
        public GameObject _ragdollCapsule;

        private LevelManager _lvlMng;
        public string nickname;
        public Rigidbody pelvisRb;
        public Animator anim;
        private Color _color;
        private Renderer[] _allMyRenderers;
        [Tooltip("Radio que va a tener el jugador para comprobar cosas como cuantos amigos tiene alrededor, etc")]
        public float contactRadius = 4f;
        public float speed = 60f;
        public float jumpSpeed = 200f;
        public float rotationSpeed = 2f;
        public float cameraSpeed = 0.6f;
        [Tooltip("Distancia que vamos a necesitar estar del piso para poder saltar.")]
        public float inAirDistance = 0.6f;
        public float minFOV;
        public float maxFOV;
        [Tooltip("Offset de la camara con respecto al character")]
        public Vector3 cameraOffset = new Vector3(-0.01f, 5.9f, -4f);
        [Tooltip("Mientras mas bajo, mas va a quedar en el MinFoV. Caso contrario, del MaxFoV.")]
        public float ratioMultiplierFoV;
        public float sqrMagnitudeInTimeSpeed;
        [Tooltip("Altura minima en la que el player debe volver")]
        public float heightRespawn = -3f;

        public event Action<int> OnPointsUpdate = delegate { }; //se llama cada vez que ganamos o perdemos puntos
        public event Action OnJump = delegate { }; //se llama cada vez que saltamos
        public event Action<bool> OnCrowned = delegate { }; //se llama cuando agarramos la corona o perdemos la corona
        public Func<int> GetActiveModeValue; //Va a conseguir el valor importante del modo de juego actual (amigos, puntos, etc)

        private void Awake()
        {
            _lvlMng = FindObjectOfType<LevelManager>();

            _allMyRenderers = GetComponentsInChildren<Renderer>();

            var characterView = new CharacterView(this);
            _allUpdatables.Add(characterView);
            _allConstructables.Add(characterView);

            if (!photonView.IsMine) return;

            var allChilds = GetComponentsInChildren<Transform>();
            allChilds.Select(x =>
            {
                x.gameObject.layer = Layers.PLAYER;
                _ragdollCapsule.layer = Layers.RAGDOLL;
                return x;
            }).ToList();

            anim = GetComponent<Animator>();

            _allUpdatables.Add(new CharacterController(this));
            _movementController = new CharacterMovement(this, pelvisRb, pelvisRb.transform.localRotation);
            _allConstructables.Add(_movementController);
            _allUpdatables.Add(_movementController);
            _allUpdatables.Add(new CharacterCamera(this, pelvisRb));
            _allUpdatables.Add(new CharacterPointsManager(this, _lvlMng, PhotonNetwork.NickName));
            _allUpdatables.Add(new CharacterFriendsManager(this, _lvlMng.playerFriendsLayermask));

            var colorA = _allMyRenderers[0].material.GetColor("_ColorA");
            var colorB = _allMyRenderers[0].material.GetColor("_ColorB");
            var colorC = _allMyRenderers[0].material.GetColor("_ColorC");
            photonView.RPC("RPCUpdateColor", RpcTarget.AllBuffered,
                new float[] { colorA.r, colorA.g, colorA.b },
                new float[] { colorB.r, colorB.g, colorB.b },
                new float[] { colorC.r, colorC.g, colorC.b });

            ArtificialAwakes();
        }

        private void OnDrawGizmos()
        {
            if(pelvisRb != null)
                Gizmos.DrawWireSphere(pelvisRb.transform.position, contactRadius);
        }

        public void UpdatePoints(int addedPoints) => OnPointsUpdate(addedPoints);
        public void Crowned(bool on) => OnCrowned(on);
        public void TryJump() { if (_movementController.inAir) return; OnJump(); }

        private void Start() { if (!photonView.IsMine) return; ArtificialStart(); }
        private void Update() { if (!photonView.IsMine && pelvisRb != null) return; ArtificialUpdate(); }
        private void FixedUpdate() { if (!photonView.IsMine) return; ArtificialFixedUpdate(); }
        private void LateUpdate() { if (!photonView.IsMine) return; ArtificialLateUpdate(); }

        [PunRPC]
        public void RPCUpdateColor(float[] colorA, float[] colorB, float[] colorC)
        {
            _allMyRenderers.Select(x =>
            {
                x.material.SetColor("_ColorA", new Color(colorA[0], colorA[1], colorA[2]));
                x.material.SetColor("_ColorB", new Color(colorB[0], colorB[1], colorB[2]));
                x.material.SetColor("_ColorC", new Color(colorC[0], colorC[1], colorC[2]));
                return x;
            }).ToList();
        }
        public void ArtificialAwakes() { _allConstructables.Select(x => { x.ArtificialAwake(); return x; }).ToList(); }
        public void ArtificialStart() { _allConstructables.Select(x => { x.ArtificialStart(); return x; }).ToList(); }
        public void ArtificialUpdate() { _allUpdatables.Select(x => { x.ArtificialUpdate(); return x; }).ToList(); CheckHeight(); }
        public void ArtificialFixedUpdate() { _allUpdatables.Select(x => { x.ArtificialFixedUpdate(); return x; }).ToList(); }
        public void ArtificialLateUpdate() { _allUpdatables.Select(x => { x.ArtificialLateUpdate(); return x; }).ToList(); }

        void CheckHeight()
        {
            if (pelvisRb.transform.position.y < heightRespawn)
                _lvlMng.RespawnRandom(pelvisRb.transform);
        }

        public void AddPoint(int points)
        {
            _lvlMng.UpdateUserPoints(PhotonNetwork.NickName, points);

        }
    }

}
