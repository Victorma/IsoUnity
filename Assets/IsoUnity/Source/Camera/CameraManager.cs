using UnityEngine;
using System.Collections;
using IsoUnity.Entities;

namespace IsoUnity
{
    public class CameraManager : EventedEventManager
    {
        public enum CameraMode
        {
            Normal, Smooth
        }

        /*
         * Static main game instance
         */
        private static CameraManager instance = null;
        public static CameraManager Instance
        {
            get
            {
                // Singleton or find or create
                return instance ? instance : ((instance = FindObjectOfType<CameraManager>()) ? instance : instance = new GameObject("CameraManager").AddComponent<CameraManager>());
            }
        }

        [SerializeField]
        private CameraMode cameraMode;
        [SerializeField]
        private bool initializeCameraOnStart = true;
        [SerializeField]
        private GameObject look = null;

        private static float distance = 30;
        private static Vector3 separation;

        private Vector3 speed;
        private float distanceToTarget = 0;
        private IGameEvent lookEvent;

        #region properties

        /*
         *  Looking things (Recommended to use CameraManager.lookTo(<<Target>>) to manage camera).
         */
        public GameObject Target { get { return look; } }

        public CameraMode Mode { get { return cameraMode; } set { cameraMode = value; } }

        public Vector3 cameraPosition
        {
            get { return Camera.main.transform.position; }
            set { Camera.main.transform.position = value; }
        }

        #endregion

        void Start()
        {
            if (Camera.main == null)
            {
                GameObject camera = new GameObject();
                camera.AddComponent<Camera>();
                camera.AddComponent<AudioListener>();
                camera.name = "MainCamera";
                camera.tag = "MainCamera";
            }

            if (initializeCameraOnStart)
            {
                Camera.main.orthographic = true;
                
                Texture2D baseTile = IsoSettingsManager.getInstance().getIsoSettings().defautTextureScale;

                float angle = 30;
                if (baseTile != null)
                {
                    float angulo = Mathf.Rad2Deg * Mathf.Acos(baseTile.height / (baseTile.width * 1f));
                    angle = 90f - angulo;
                }
                Camera.main.transform.rotation = Quaternion.Euler(angle, 45, 0);

                separation = new Vector3(0, 0, distance);
                separation = Quaternion.Euler(angle, 45, 0) * separation;
                speed = Vector3.zero;

            }

            if (this.look == null)
            {

                Player player = FindObjectOfType<Player>();

                if (player != null)
                {
                    this.look = player.gameObject;
                    MapManager.getInstance().setActiveMap(player.Entity.Position.Map);
                }
            }

            Flush();

            RenderSettings.ambientLight = Color.black;
        }

        void Update()
        {


            if (look != null)
            {
                if (Mode == CameraMode.Smooth)
                {
                    Vector3 destination = (look.transform.position - separation);
                    Vector3 origin = Camera.main.transform.position;


                    Camera.main.transform.position = Vector3.SmoothDamp(origin, destination, ref speed, .5f);
                    distanceToTarget = (Camera.main.transform.position - destination).magnitude;

                    /*
                    Vector3 movement = destination - origin;
                    float space = movement.magnitude;
                    if ((Camera.main.transform.position - destination).magnitude > 0.02)
                        space += 1;
                    
                    float acceleration;
                    acceleration = space * space;
                    Vector3 direction = movement.normalized;

                    Vector3 move = acceleration * Time.deltaTime * direction;

                    Camera.main.transform.position = Camera.main.transform.position + move;*/
                }
                else
                {
                    Camera.main.transform.position = look.transform.position - separation;
                    distanceToTarget = 0;
                }
            }

        }

        [GameEvent(true, false)]
        public IEnumerator LookTo(GameObject gameObject, bool instant = false)
        {
            var currentEvent = lookEvent = Current;

            look = gameObject;
            distanceToTarget = float.MaxValue;

            if (instant)
                Flush();

            yield return new WaitUntil(() => currentEvent != lookEvent || distanceToTarget < .1f);
        }

        [GameEvent(true, false)]
        public void SetCameraMode(CameraMode mode)
        {
            this.Mode = mode;
        }

        public void Flush()
        {

            if (look != null)
            {
                Vector3 destination = (look.transform.position - separation);
                Camera.main.transform.position = destination;
                distanceToTarget = 0;
            }
        }
    }
}