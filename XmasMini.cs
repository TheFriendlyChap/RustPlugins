using UnityEngine;
using Newtonsoft.Json;
using System.Linq;
using System;

namespace Oxide.Plugins
{
    [Info("Xmas Mini", "The Friendly Chap", "1.0.3")]
    [Description("Spawns christmas lights on the minicopter. Merry X-Mas.")]
    class XmasMini : RustPlugin
    {
        #region Defines
        const string prefabName = "assets/prefabs/misc/xmas/christmas_lights/xmas.lightstring.deployed.prefab";
        private static readonly Vector3 prefabPosition = new Vector3(0.08f, 0.21f, 0.6f);
        private static readonly Quaternion prefabRotation = Quaternion.Euler(180, 90, 180);
        private static readonly Vector3 prefabPosition2 = new Vector3(0.0f, 0.65f, -1.2f);
        private static readonly Quaternion prefabRotation2 = Quaternion.Euler(180, 90, 178);
        #endregion

        #region Hooks
        void OnEntitySpawned(MiniCopter mini) => Setup(mini);
        #endregion

        #region Functions
        public void Setup(MiniCopter mini)
        {
            SpawnLights(mini, prefabPosition, prefabRotation);
            SpawnLights(mini, prefabPosition2, prefabRotation2);
        }

        void SpawnLights(MiniCopter mini, Vector3 position, Quaternion rotation)
        {
            BaseEntity entity = GameManager.server.CreateEntity(prefabName, mini.transform.position, rotation);
            if (entity == null) return;
            entity.SetParent(mini);
            entity.transform.localPosition = position;
            entity.Spawn();
        }
        #endregion
    }
}