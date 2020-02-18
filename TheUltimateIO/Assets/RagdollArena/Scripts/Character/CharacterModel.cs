using GameUI;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Weapons;
using Construction;

namespace Character
{
    public class CharacterModel : MonoBehaviourPun, IDrunk, IDamageable
    {
        private List<IUpdatable> _allUpdatables = new List<IUpdatable>();
        private List<IConstructable> _allConstructables = new List<IConstructable>();
        private CharacterMovement _movementController;
        public Image barLife;

        public GameObject model;
        public float forceImpulseDamage;
        public ParticlesPlayer particlesPlayer;

        private LevelManager _lvlMng;
        public string nickname;
        public Rigidbody rb;

        public Animator anim;
        private Color _color;
        private Renderer[] _allMyHeads;
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
        public CharacterCamera characterCamera;

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
        private CharacterBody _characterBody;

        Server _server;
        private bool _controlsActive = true;

        [Tooltip("Armas: ")]
        public LayerMask layerMaskWeaponDamage;
        public Animator animArms;
        CharacterWeapon _characterWeapon;

        Action _Update, _FixedUpdate, _LateUpdate;
        Action<float, float> _Move;

        [HideInInspector] public int team = 0; // { 0 } = sin equipo. { 1, 2, 3, 4 } = posibles equipos que pueden haber.
        [Tooltip("Este owned es parecido al photonView.isMine, solo que es para FullAutho, ya que el server es el photonView.isMine")] public bool owned;

        [PunRPC] public void RPCSetModelOwner(bool own) => owned = own;

        [PunRPC] public void RPCArtificialAwake()
        {

            _lvlMng = FindObjectOfType<LevelManager>();

            _characterBody = FindObjectOfType<CharacterBody>();

            _allMyHeads = _characterBody._allHeadParts.Select(x => x.GetComponentInChildren<Renderer>()).ToArray();

            characterStats = GetComponent<CharacterStats>();

            var characterView = new CharacterView(this);
            _allUpdatables.Add(characterView);
            _allConstructables.Add(characterView);

            _movementController = new CharacterMovement(this, rb, rb.transform.localRotation, floorLayers);
            _allConstructables.Add(_movementController);
            _allUpdatables.Add(_movementController);
            _Move = _movementController.Move;

            characterCamera = new CharacterCamera(this, rb);
            _allUpdatables.Add(characterCamera);

            var weaponsUIManager = FindObjectOfType<WeaponsAndStatsUIManager>();
            var WeaponsManager = GetComponentInChildren<WeaponsManager>();
            var chat = FindObjectOfType<Chat>();

            _characterWeapon = new CharacterWeapon(this, weaponsUIManager, WeaponsManager, chat);
            _allUpdatables.Add(_characterWeapon);

            _hp = characterStats.life;

            particlesPlayer = GetComponentInChildren<ParticlesPlayer>();

            _allUpdatables.Add(FindObjectOfType<Controller>());

            if (!owned) return;

            _Update = ArtificialUpdate;
            _FixedUpdate = ArtificialFixedUpdate;
            _LateUpdate = ArtificialLateUpdate;

            chat.InitializedChat(this);
            chat.SuscribeChat(ChatActive);

            Debug.Log("<color=green> Paso por aca porque es owner. ArtificialAwake </color>");

            FindObjectOfType<ConstructionPanel>().OnConstructionMode += GetComponentInChildren<WeaponsManager>().ConstructionMode;

            _allUpdatables.Add(new CharacterPointsManager(this, _lvlMng, PhotonNetwork.NickName));
            _allUpdatables.Add(new CharacterFriendsManager(this, _lvlMng.playerFriendsLayermask));

            photonView.RPC("RPCUpdateColorTeamAndHead", RpcTarget.AllBuffered,
                new float[] { PlayerPrefs.GetFloat("SkinColorR"), PlayerPrefs.GetFloat("SkinColorG"), PlayerPrefs.GetFloat("SkinColorB") },
                new float[] { Color.grey.r, Color.grey.g, Color.grey.b },
                PlayerPrefs.GetInt("HeadTypeID"), 0);

            ChangeTeam(1);

            ArtificialAwakes();
        }

        [PunRPC] public void RPCChangePlayerTeam(int teamID) => ChangeTeam(teamID);
        private void ChangeTeam(int ID) //cambiar el team equivale tambien a cambiar el color del jugador y color de efectos
        {
            Debug.Log("<color=green> Fuiste cambiado al equipo " + ID.ToString() + "</color>");
            team = ID-1;

            Color teamIDColor = ID == 0 ? Color.grey : ID == 1 ? Color.blue : ID == 2 ? Color.red : ID == 3 ? Color.yellow : Color.blue;

            photonView.RPC("RPCUpdateColorTeamAndHead", RpcTarget.AllBuffered,
                new float[] { PlayerPrefs.GetFloat("SkinColorR"), PlayerPrefs.GetFloat("SkinColorG"), PlayerPrefs.GetFloat("SkinColorB") },
                new float[] { teamIDColor.r, teamIDColor.g, teamIDColor.b },
                PlayerPrefs.GetInt("HeadTypeID"), team);
        }

        [PunRPC] public void RPCStartGame()
        {
            _lvlMng.gameCanvas.SwitchMapPanel(true);
            _lvlMng.gameCanvas.SwitchCounterPanel(false);
            Debug.Log("<color=yellow> GO!!! </color>");
            //TODO: primero aca hacer efecto de teletransportarse o lo que sea, junto con sonidos, etc.
            //TODO: Abrir panel de mapa junto a todo lo que tenga
        }

        void ChatActive(bool active)
        {
            _controlsActive = active;
        }

        private void OnDrawGizmos()
        {
            if (rb != null)
                Gizmos.DrawWireSphere(rb.transform.position, contactRadius);
        }
        public void UpdatePoints(int addedPoints) => OnPointsUpdate(addedPoints);
        public void Crowned(bool on) => OnCrowned(on);
        public void TryJump() { if (_movementController.inAir) return; OnJump(); }
        private void Start() { if (!owned) return; ArtificialStart(); }
        private void Update() { if ((!owned && rb != null) || !_controlsActive) return; _Update(); }
        private void FixedUpdate() { if (!owned || !_controlsActive) return; _FixedUpdate(); }
        private void LateUpdate() { if (!owned || !_controlsActive) return; _LateUpdate(); }
        [PunRPC] public void RPCUpdateColorTeamAndHead(float[] skinColor, float[] teamColor, int headTypeID, int teamTypeID)
        {
            _allMyHeads.Select(x =>
            {
                x.material.SetColor("_SkinColor", new Color(skinColor[0], skinColor[1], skinColor[2], 1f));
                return x;
            }).ToList();
            _characterBody.SelectHead(headTypeID);
            _characterBody.SelectBody(teamTypeID);

            _characterWeapon.UpdateWeaponColors(teamColor[0], teamColor[1], teamColor[2]);
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
            model.SetActive(!dead);
            OnChangeRespawnMode(dead ? CharacterCamera.CameraMode.GodMode : CharacterCamera.CameraMode.ThirdPersonMode);

            if (!owned) return;

            _lvlMng.gameCanvas.SwitchRespawnHUD(dead);
            _lvlMng.gameCanvas.SwitchMapPanel(dead);
            if (dead)
                FindObjectOfType<SpawnMap>().SetSpawnPointer();
           // else
                //Damage(transform.position, -1f);
            Debug.Log(dead ? "<color=red>Muerto</color>" : "<color=green>Respawneado</color>");
        }
        public void RespawnAtPosition(Vector3 positionToRespawn)
        {
            rb.transform.position = positionToRespawn;
            rb.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
        void CheckHeight()
        {
            if (rb.transform.position.y < heightRespawn)
                _lvlMng.RespawnRandom(rb.transform);
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
            _Move(horizontal, vertical);
        }

        public void Damage(Vector3 origin, float damage)
        {
            if (!FindObjectOfType<Server>().DamageActive()) return;

            rb.AddForce((rb.transform.position - origin) * forceImpulseDamage, ForceMode.Impulse);
            _hp -= damage;

            photonView.RPC("RPCDamage", RpcTarget.All, _hp / characterStats.life);
        }
        [PunRPC] public void RPCDamage(float hp)
        {
            particlesPlayer.particlesDamage.Play();

            barLife.fillAmount = hp;

            if (!owned) return;

            if (hp <= 0)
                FindObjectOfType<Server>().photonView.RPC("RPCPlayerDeath", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer);
        }

        [PunRPC] public void RPCActivateEmoji(int emojiID)
        {
            particlesPlayer.particlesEmoji[emojiID].Play();
        }

        [PunRPC]
        public void RPCActivateChat(bool activateChat)
        {
            if(activateChat)
                particlesPlayer.particleChat.Play();
            else
                particlesPlayer.particleChat.Stop();
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
                rb.transform.position + transform.forward * 2f, Quaternion.identity);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit, Mathf.Infinity, floorLayers);
            grenade.GetComponent<Rigidbody>()
                .AddForce((hit.point - rb.transform.position + Vector3.up * grenadeYThrowSpeed).normalized
                * _grenadeThrowSpeed * Time.deltaTime, ForceMode.Impulse);
        }
        #endregion
        #region RPCWeapons
        [PunRPC] void RPCAnimSword() { animArms.SetTrigger("SwordAttack"); }
        [PunRPC] void RPCAnimBow() { animArms.SetTrigger("BowAttack"); }
        [PunRPC]
        public void RPCChangeWeapon(int selectedWeaponID)
        {
            _characterWeapon.weaponsMng.ChangeWeapon(selectedWeaponID);
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
                rb.transform.position + (transform.forward * 2f) + (transform.up * 3f), Quaternion.identity);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit, Mathf.Infinity, floorLayers);
            grenadeDrunk.GetComponent<Rigidbody>()
                .AddForce((hit.point - rb.transform.position).normalized
                * 20, ForceMode.Impulse);
        }
        #endregion
        #region Controls
        public void ChangeControls(Action u, Action fu, Action lu, Action<float, float> move)
        {
            _Update = u;
            _FixedUpdate = fu;
            _LateUpdate = lu;
            _Move = move;
        }
        public void ChangeControls(Action u, Action fu, Action lu, Action<float, float> move, Rigidbody newRb)
        {
            _Update = u;
            _FixedUpdate = fu;
            _LateUpdate = lu;
            _Move = move;
            characterCamera.ChangeTarget(newRb);
        }

        public void NormalControls()
        {
            _Update = ArtificialUpdate;
            _FixedUpdate = ArtificialFixedUpdate;
            _LateUpdate = ArtificialLateUpdate;
            _Move = _movementController.Move;
            characterCamera.ChangeTarget(rb);
        }
        #endregion
    }
}
