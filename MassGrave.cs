using Newtonsoft.Json;
using ProtoBuf;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Mass Grave", "The Friendly Chap", "1.0.2")]
    [Description("Teleports Player Bodies to a set position on the map.")]
    class MassGrave : RustPlugin
    {
        #region ConfigData
        private ConfigData configData;

        class ConfigData
        {
            [JsonProperty(PropertyName = "Mass Grave Position (you can use client.printpos ingame to get position)")]
            public Vector3 GraveHole;

            [JsonProperty(PropertyName = "Mass Grave Location Name")]
            public string GraveLocation = "Outpost";
        }
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
            if (!LoadConfigVariables())
            {
                Puts("Config file issue detected. Please delete file, or check syntax and fix.");
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
        void OnPlayerCorpseSpawned(BasePlayer player, BaseCorpse corpse)
        {
            if (configData.GraveHole == null) return;
            SendReply(player, $"<color=#FFOOOO>A corpse for {player.displayName} has just been spawned, at the Mass Grave {configData.GraveLocation}</color>");
            corpse.transform.position = configData.GraveHole;
            corpse.SendNetworkUpdate();
        }


    }
}
