using UnityEngine;
using System.Collections.Generic;
using IsoUnity.Entities;

namespace IsoUnity
{
    public class Game : MonoBehaviour
    {

        /**
         * This var allows new Game instances load destroy the previous one and replace it.
         * This can have unexpected behaviours, we recommend to use only one Game class along
         * all the game execution.
         */
        public bool shouldReplacePreviousGame = false;

        Queue<IGameEvent> events;

        /*
         *  Looking things (Recommended to use CameraManager.lookTo(<<Target>>) to manage camera).
         */
        public GameObject look; // Do not use this to know who's camera looking, WRITE ONLY (Use CameraManager instead)
        private GameObject previousLook;

        /*
         * Initial Map of the game (Recommended to use MapManager to manage maps).
         */
        public Map map; // Do not use this to know what map is active, WRITE ONLY (Use MapManager instead)
        private Map previousMap;

        /*
         * Event Manager Things
         * Use this list to create the managers at the start of the game.
         * (By default Animation, Secuence and IsoSwitches Managers are created).
         */
        private List<EventManager> eventManagers;


        /*
         * Screen controls default controller.
         */
        public bool onScreenControls;

        /*
         * Static main game instance
         */
        private static Game m;
        public static Game main
        {
            get
            {
                if (m == null)
                {
                    m = FindObjectOfType<Game>();
                    if(m!=null) m.Awake();
                }
                return m;
            }
        }

        /*
         * Game initialization
         */
        private bool awakened = false;
        void Awake()
        {
            if (awakened)
                return;
            awakened = true;

            if (Game.main != this)
            {
                if (shouldReplacePreviousGame)
                {
                    GameObject.DestroyImmediate(Game.main.gameObject);
                }
                else
                {
                    if (Game.main != null)
                    {
                        GameObject.DestroyImmediate(this.gameObject);
                        return;
                    }
                }
            }

            Game.m = this;
            if (Application.isPlaying)
                GameObject.DontDestroyOnLoad(this.gameObject);

            // Event Queue
            events = new Queue<IGameEvent>();

            // Main Managers initialization
            // TODO Make they event managers as the rest

            if (this.look == null)
            {
                Player player = FindObjectOfType<Player>();
                if (player != null)
                {
                    this.look = player.gameObject;
                    this.map = player.Entity.Position.Map;
                }
            }

            CameraManager.initialize();
            CameraManager.lookTo(look);
            MapManager.getInstance().hideAllMaps();
            MapManager.getInstance().setActiveMap(map);
            ControllerManager.Enabled = true;
            IsoSwitchesManager.getInstance().getIsoSwitches();

            // Event Managers Creation
            eventManagers = new List<EventManager>();

            // On Screen Controlls creation
            // TODO move this to GUI Manager as it becomes a regular EventManager
            if (this.onScreenControls)
                GUIManager.addGUI(new OnScreenControlsGUI(), 99);
        }

        void Update()
        {
            this.tick();
        }

        void OnGUI()
        {
            GUIManager.tick();
        }

        /*
         * Event methods
         */

        public void enqueueEvent(IGameEvent ge)
        {
            if (ge == null)
                return;
            this.events.Enqueue(ge);
        }

        public void eventFinished(IGameEvent ge, Dictionary<string, object> extraParameters = null)
        {
            object sync = ge.getParameter("synchronous");
            if (sync != null && ((bool)sync))
            {
                GameEvent f = new GameEvent();
                f.Name = "event finished";
                f.setParameter("event", ge);
                // Put the extra parameters
                if (extraParameters != null) foreach (var kv in extraParameters) f.setParameter(kv.Key, kv.Value);
                this.enqueueEvent(f);
            }
        }

        // Private method used to broadcast the events in main tick
        private void broadcastEvent(IGameEvent ge)
        {
            foreach (EventManager manager in eventManagers)
                manager.ReceiveEvent(ge);

            foreach (Map map in MapManager.getInstance().getMapList())
                map.broadcastEvent(ge);
        }

        /*
         * As the player input isnt so frecuent, it's only checked each ms to improve performance
         */
        private float timeToController = 100 / 1000;
        private float currentTimeToController = 0;

        public void tick()
        {
            // Variable changes
            if (previousLook != look)
                CameraManager.lookTo(previousLook = look);

            if (previousMap != map)
                MapManager.getInstance().setActiveMap(previousMap = map);

            // Main Tick
            CameraManager.Update();

            // Controller management
            currentTimeToController += Time.deltaTime;
            if (currentTimeToController > timeToController)
            {
                ControllerManager.tick();
                currentTimeToController -= timeToController;
            }

            // Events launch
            while (events.Count > 0)
            {
                IGameEvent ge = events.Dequeue();
                broadcastEvent(ge);
            }

            // EventManagers ticks
            foreach (EventManager manager in eventManagers)
                manager.Tick();

            // Entities ticks
            foreach (Map eachMap in MapManager.getInstance().getMapList())
            {
                eachMap.tick();
            }
        }

        /**
         * EventManager management
         **/

        public void RegisterEventManager(EventManager em)
        {
            if (!this.eventManagers.Contains(em))
                this.eventManagers.Add(em);
        }

        public void DeRegisterEventManager(EventManager em)
        {
            if (this.eventManagers.Contains(em))
                this.eventManagers.Remove(em);
        }

    }
}