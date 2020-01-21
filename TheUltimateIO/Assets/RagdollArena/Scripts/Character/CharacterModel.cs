using GameUI;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Character
{
    public class CharacterModel : MonoBehaviourPun, IDrunk, IDamageable
    {
        private List<IUpdatable> _allUpdatables = new List<IUpdatable>();
        private List<IConstructable> _allConstructables = new List<IConstructable>();
        private CharacterMovement _movementController;
        public GameObject _ragdollCapsule;
        public Image barLife;

        private LevelManager _lvlMng;
        public string nickname;
        public Rigidbody pelvisRb;

        public Animator anim;
        private Color _color;
        private Renderer[] _allMyRenderers;
        public float _hp;
        [Tooltip("Radio que va a tener el jugador para comprobar cosas como cuantos amigos tiene alrededor, etc")]
        public float contactRadius = 4f;
        [HideInInspector] public CharacterStats characterStats;

        //Camara
        public float cameraSpeed = 0.6f;
        [Tooltip("Distancia que vamos a necesitar estar del piso para poder saltar.")]
        public float inAirDistance = 0.6f;
        public float minFOV;
        public float maxFOV;
        [Tooltip("Offset de la camara con respecto a la pelvis en third person")]
        public Vector3 thirdPersonCameraOffset = new Vector3(-0.01f, 5.9f, -4f);
        [Tooltip("Offset de posicion y rotacion de la camara con respecto al character en god mode")]
        public Transform godModeCameraOffset;
        [Tooltip("Mientras mas bajo, mas va a quedar en el MinFoV. Caso contrario, del MaxFoV.")]
        public float ratioMultiplierFoV;
        [Tooltip("Cercania a la que vamos a estar para que la camara se empiece a alejar de los nexos")]
        public float coreDistancingCloseness;
        [Tooltip("Velocidad en la que se va a alejar la camara cerca de un nexo")]
        public float coreDistancingSpeed;
        [Tooltip("Distancia a la que se va a alejar la camara cerca de un nexo")]
        public float coreDistancingAmount;

        public float sqrMagnitudeInTimeSpeed;
        [Tooltip("Altura minima en la que el player debe volver")]
        public float heightRespawn = -3f;
        [Tooltip("Fuerza para lanzar cosas")]
        public float pushForce;
        [Tooltip("Velocidad de lanzamiento de granada")]
        public float grenadeThrowSpeed;
        private float _grenadeThrowSpeed;
        [Tooltip("Cuanto para arriba va a tirar las granadas")]
        public float grenadeYThrowSpeed;
        [Tooltip("Este va a ser la velocidad a la que va a subir el grenadethrowspeed cuando apretamos")]
        public float grenadeThrowSpeedThreshold;
        public LayerMask floorLayers;

        [Tooltip("Tiempo que dura la borrachera")]
        public float timeDrunk;
        public ParticleSystem particlesDrunk;
        bool _drunkActive;
        float _counterDrunk;
        public event Action<int> OnPointsUpdate = delegate { }; //se llama cada vez que ganamos o perdemos puntos
        public event Action OnJump = delegate { }; //se llama cada vez que saltamos
        public event Action<bool> OnCrowned = delegate { }; //se llama cuando agarramos la corona o perdemos la corona
        public Func<int> GetActiveModeValue; //Va a conseguir el valor importante del modo de juego actual (amigos, puntos, etc)

        public List<CharacterHands> hands = new List<CharacterHands>();

        Server _server;

        [HideInInspector] public int team = 0; // { 0 } = sin equipo. { 1, 2, 3, 4 } = posibles equipos que pueden haber.
        [Tooltip("Este owned es parecido al photonView.isMine, solo que es para FullAutho, ya que el server es el photonView.isMine")] public bool owned;

        [PunRPC] public void RPCSetModelOwner(bool own) => owned = own;

        [PunRPC] public void RPCArtificialAwake()
        {
            _lvlMng = FindObjectOfType<LevelManager>();

            _allMyRenderers = GetComponentsInChildren<Renderer>();

            characterStats = GetComponent<CharacterStats>();

            var characterView = new CharacterView(this);
            _allUpdatables.Add(characterView);
            _allConstructables.Add(characterView);

            _movementController = new CharacterMovement(this, pelvisRb, pelvisRb.transform.localRotation, floorLayers);
            _allConstructables.Add(_movementController);
            _allUpdatables.Add(_movementController);

            _hp = characterStats.life;

            if (!owned) return;
            Debug.Log("<color=green> Paso por aca porque es owner. ArtificialAwake </color>");

            FindObjectOfType<Chat>().characterModel = this; //Le aviso quien soy al chat

            var allChilds = GetComponentsInChildren<Transform>();
            //var damageable = GetComponentInChildren<Damageable>();
            allChilds.Select(x =>
            {
                x.gameObject.layer = Layers.PLAYER;
                _ragdollCapsule.layer = Layers.RAGDOLL;
                //damageable.gameObject.layer = Layers.DAMAGEABLE;
                return x;
            }).ToList();

            anim = GetComponent<Animator>();

            _allConstructables.Add(GetComponentInChildren<CharacterHands>());
            _allUpdatables.Add(GetComponentInChildren<CharacterHands>());
            _allUpdatables.Add(new CharacterCamera(this, pelvisRb));
            _allUpdatables.Add(new CharacterPointsManager(this, _lvlMng, PhotonNetwork.NickName));
            _allUpdatables.Add(new CharacterFriendsManager(this, _lvlMng.playerFriendsLayermask));

            var colorA = _allMyRenderers[1].material.GetColor("_ColorA");
            var colorB = _allMyRenderers[1].material.GetColor("_ColorB");
            var colorC = _allMyRenderers[1].material.GetColor("_ColorC");
            photonView.RPC("RPCUpdateColor", RpcTarget.AllBuffered,
                new float[] { colorA.r, colorA.g, colorA.b },
                new float[] { colorB.r, colorB.g, colorB.b },
                new float[] { colorC.r, colorC.g, colorC.b });

            ChangeTeam(0);

            ArtificialAwakes();
        }

        [PunRPC] public void RPCChangePlayerTeam(int teamID) => ChangeTeam(teamID);
        private void ChangeTeam(int ID) //cambiar el team equivale tambien a cambiar el color del jugador y color de efectos
        {
            Debug.Log("<color=green> Fuiste cambiado al equipo " + ID.ToString() + "</color>");
            team = ID;

            Color previousColorA = _allMyRenderers[1].material.GetColor("_ColorA");
            Color previousColorC = _allMyRenderers[1].material.GetColor("_ColorC");
            Color newCol = ID == 0 ? Color.grey : ID == 1 ? Color.blue : ID == 2 ? Color.red : ID == 3 ? Color.yellow : Color.blue;

            photonView.RPC("RPCUpdateColor", RpcTarget.AllBuffered,
                new float[] { previousColorA.r, previousColorA.g, previousColorA.b },
                new float[] { newCol.r, newCol.g, newCol.b },
                new float[] { previousColorC.r, previousColorC.g, previousColorC.b });
        }

        [PunRPC] public void RPCStartGame()
        {
            _lvlMng.gameCanvas.SwitchMapPanel(true);
            _lvlMng.gameCanvas.SwitchCounterPanel(false);
            Debug.Log("<color=yellow> GO!!! </color>");
            //TODO: primero aca hacer efecto de teletransportarse o lo que sea, junto con sonidos, etc.
            //TODO: Abrir panel de mapa junto a todo lo que tenga
        }

        private void OnDrawGizmos()
        {
            if (pelvisRb != null)
                Gizmos.DrawWireSphere(pelvisRb.transform.position, contactRadius);
        }
        public void UpdatePoints(int addedPoints) => OnPointsUpdate(addedPoints);
        public void Crowned(bool on) => OnCrowned(on);
        public void TryJump() { if (_movementController.inAir) return; OnJump(); }
        private void Start() { if (!owned) return; ArtificialStart(); }
        private void Update() { if (!owned && pelvisRb != null) return; ArtificialUpdate(); }
        private void FixedUpdate() { if (!owned) return; ArtificialFixedUpdate(); }
        private void LateUpdate() { if (!owned) return; ArtificialLateUpdate(); }
        [PunRPC] public void RPCUpdateColor(float[] colorA, float[] colorB, float[] colorC)
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

        public event Action<CharacterCamera.CameraMode> OnChangeRespawnMode = delegate { };
        [PunRPC] public void RPCRespawn(Vector3 positionToRespawn)
        {
            RPCChangeRespawnMode(false); //no hace falta llamarlo desde RPC aca porque ya estamos en la misma
            RespawnAtPosition(positionToRespawn);
        }
        [PunRPC] public void RPCChangeRespawnMode(bool dead)
        {
            pelvisRb.transform.parent.gameObject.SetActive(!dead);
            OnChangeRespawnMode(dead ? CharacterCamera.CameraMode.GodMode : CharacterCamera.CameraMode.ThirdPersonMode);

            if (!owned) return;

            _lvlMng.gameCanvas.SwitchRespawnHUD(dead);
            _lvlMng.gameCanvas.SwitchMapPanel(dead);
            if (dead)
                FindObjectOfType<SpawnMap>().SetSpawnPointer();
            else
                Damage(-1f);
            Debug.Log(dead ? "<color=red>Muerto</color>" : "<color=green>Respawneado</color>");
        }
        public void RespawnAtPosition(Vector3 positionToRespawn) => pelvisRb.transform.position = positionToRespawn;

        void CheckHeight()
        {
            if (pelvisRb.transform.position.y < heightRespawn)
                _lvlMng.RespawnRandom(pelvisRb.transform);
        }

        public void AddPoint(int points)
        {
            var cubeSpawner = FindObjectOfType<CubeSpawner>();
            if(cubeSpawner != null)
                cubeSpawner.ConstructionPoints += points;
        }

        public bool OnClickPlayer()
        {
            foreach (var item in hands)
                if (item.activeTaken) 
                    return true;
            return false;
        }
        public void MovePlayer(float horizontal, float vertical)
        {
            _movementController.Move(horizontal, vertical);
        }

        public void Damage(float damage)
        {
            if (!FindObjectOfType<Server>().DamageActive()) return;

            _hp -= damage;

            photonView.RPC("RPCDamage", RpcTarget.All, _hp / characterStats.life);
        }
        [PunRPC] public void RPCDamage(float hp)
        {
            barLife.fillAmount = hp;
            if (!owned) return;

            if(hp <= 0)
                FindObjectOfType<Server>().photonView.RPC("RPCPlayerDeath", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer);
        }

        public void Explosion(Vector3 origin, float force) { throw new NotImplementedException(); }
        #region Grenade
        public void TryGrenade()
        {
            if (_throwGrenadeCoroutine != null)
                StopCoroutine(_throwGrenadeCoroutine);
            _throwGrenadeCoroutine = StartCoroutine(ThrowGrenadeCoroutine());
        }
        private Coroutine _throwGrenadeCoroutine;
        IEnumerator ThrowGrenadeCoroutine()
        {
            _grenadeThrowSpeed = 0;
            while (true)
            {
                _grenadeThrowSpeed = 
                    Mathf.Clamp(_grenadeThrowSpeed + Time.deltaTime * grenadeThrowSpeedThreshold ,0.1f,grenadeThrowSpeed);
                yield return new WaitForEndOfFrame();
            }
        }
        public void ThrowGrenade()
        {
            StopCoroutine(_throwGrenadeCoroutine);

            GameObject grenade = PhotonNetwork.Instantiate("Grenade",
                pelvisRb.transform.position + transform.forward * 2f, Quaternion.identity);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit, Mathf.Infinity, floorLayers);
            grenade.GetComponent<Rigidbody>()
                .AddForce((hit.point - pelvisRb.transform.position + Vector3.up * grenadeYThrowSpeed).normalized
                * _grenadeThrowSpeed * Time.deltaTime, ForceMode.Impulse);
        }
        #endregion
        #region Drunk
        public void DrunkEffectActive()
        {
            if (!photonView.IsMine) return;

            if (!_drunkActive)
            {
                _drunkActive = true;
                StartCoroutine(Drunk());
            }
            else _counterDrunk = 0;
        }

        public void DrunkEffectDesactive()
        {
            if (!photonView.IsMine) return;

            _drunkActive = false;
            _counterDrunk = 0;
            photonView.RPC("ParticlesDrunk", RpcTarget.All, false);
            _movementController.ChangeControls(true);
        }

        IEnumerator Drunk()
        {
            var waitForEndOfFrame = new WaitForEndOfFrame();
            _movementController.ChangeControls(false);
            photonView.RPC("ParticlesDrunk", RpcTarget.All, true);
            while (_drunkActive)
            {
                _counterDrunk += Time.deltaTime;
                if (_counterDrunk >= timeDrunk)
                    _drunkActive = false;

                yield return waitForEndOfFrame;
            }
            DrunkEffectDesactive();
        }

        [PunRPC]
        void ParticlesDrunk(bool active)
        {
            if(active) particlesDrunk.Play();
            else particlesDrunk.Stop();
        }

        public void TryGrenadeDrunk()
        {
            GameObject grenadeDrunk = PhotonNetwork.Instantiate("GrenadeDrunk",
                pelvisRb.transform.position + (transform.forward * 2f) + (transform.up * 3f), Quaternion.identity);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit, Mathf.Infinity, floorLayers);
            grenadeDrunk.GetComponent<Rigidbody>()
                .AddForce((hit.point - pelvisRb.transform.position).normalized
                * 20, ForceMode.Impulse);
        }
        #endregion
    }
}
