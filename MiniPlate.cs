using Newtonsoft.Json;
using System.Linq;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Minicopter Licence Plate", "The Friendly Chap", "1.0.6")]
    [Description("Spawn a licence plate (Small Wooden Board) at the back of the minicopter, with optional permissions.")]
    class MiniPlate : RustPlugin
    {
        const string _platePrefab = "assets/prefabs/deployable/signs/sign.small.wood.prefab";
        PlateManager _manager = new PlateManager();
        private static readonly Vector3 PlatePosition = new Vector3(0.0f, 0.20f, -0.85f);
        private static readonly Quaternion PlateRotation = Quaternion.Euler(180, 0, 180);

        private ConfigData configData;
        class ConfigData
        {
            [JsonProperty(PropertyName = "Use Permissions")]
            public bool miniPerms = false;
        }
        #region ConfigFileStuff
        private bool LoadConfigVariables()
        {
            try
            {
                configData = Config.ReadObject<ConfigData>();
            }
            catch
            {
                return false;
            }
            SaveConfig(configData);
            return true;
        }

        void Init()
        {
            permission.RegisterPermission("miniplate.use", this);
            FindBabies();

            if (!LoadConfigVariables())
            {
                Puts("Config file issue detected. Please delete file, or check syntax and fix.");
            }
        }

        void FindBabies()
        {
            //			Puts("FindBabies Running");
            foreach (var playerMini in BaseNetworkable.serverEntities.OfType<MiniCopter>())
            {
                if (playerMini.HasChild(playerMini))
                {
                    //					Puts("Child Found");
                    foreach (var babyEnt in playerMini.children)
                    {
                        if (babyEnt.PrefabName == _platePrefab)
                        {
                            //							Puts("Got Prefab, destroying MeshCollider");
                            foreach (var mesh in babyEnt.GetComponentsInChildren<MeshCollider>())
                            {
                                UnityEngine.Object.DestroyImmediate(mesh);
                            }
                        }
                    }
                }
            }
        }
        protected override void LoadDefaultConfig()
        {
            Puts("Creating new config file.");
            configData = new ConfigData();
            SaveConfig(configData);
        }
        void SaveConfig(ConfigData config)
        {
            Config.WriteObject(config, true);
        }
        #endregion
        public string userID = "0";
        void OnEntitySpawned(MiniCopter mini, ConfigData config)
        {
            if (configData.miniPerms)
            {
                userID = $"{mini.OwnerID}";
                Puts($"Perms Active, mini userID {userID}");
                if (!permission.UserHasPermission(userID, "miniplate.use"))
                {
                    return;
                }
                else
                {
                    _manager.Setup(mini);
                }
                return;
            }
            else
            {
                _manager.Setup(mini);
                return;
            }
        }

        class PlateManager
        {
            public void Setup(BaseVehicle vehicle)
            {
                MakeNumplate(vehicle, PlatePosition);
            }

            void MakeNumplate(BaseVehicle vehicle, Vector3 position)
            {
                BaseEntity entity = GameManager.server.CreateEntity(_platePrefab, vehicle.transform.position, PlateRotation);
                if (entity == null) return;
                entity.SetParent(vehicle);
                entity.transform.localPosition = position;
                entity.Spawn();
                DestroyMeshCollider(entity);
            }

            void DestroyMeshCollider(BaseEntity ent)
            {
                //Makes it basically only visable since everything passes though it.
                foreach (var mesh in ent.GetComponentsInChildren<MeshCollider>())
                {
                    UnityEngine.Object.DestroyImmediate(mesh);
                    //No break if not grounded
                    UnityEngine.Object.DestroyImmediate(ent.GetComponent<DestroyOnGroundMissing>());
                    //No break if hits ground
                    UnityEngine.Object.DestroyImmediate(ent.GetComponent<GroundWatch>());
                    //Stops Decay
                    UnityEngine.Object.DestroyImmediate(ent.GetComponent<DeployableDecay>());
                }
            }
        }
    }
}