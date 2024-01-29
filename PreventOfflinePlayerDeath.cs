using System;
using Oxide.Core;
using Oxide.Core.Libraries.Covalence;
using UnityEngine;
using Newtonsoft.Json;

namespace Oxide.Plugins
{
    [Info("PreventOfflinePlayerDeath", "The Friendly Chap", "1.0.1")]
    [Description("Prevent offline players from dying and teleport them")]

    class PreventOfflinePlayerDeath : RustPlugin
    {
		private ConfigData configData;
		
class ConfigData 
{
    [JsonProperty(PropertyName = "Safe Location")]
    public Vector3 teleportLocation = new Vector3(-6.83f, 998f, -4.99f);
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
				Puts("Config error, please delete PreventOfflinePlayerDeath config file.");
				return;
			}
				
		}
		
		protected override void LoadDefaultConfig()
		{
			Puts("Creating New Config File.");
			configData = new ConfigData();
			SaveConfig(configData);
		}
		
		void SaveConfig(ConfigData config)
		{
			Config.WriteObject(config, true);
		}
		
		// Main Hooks

        object OnPlayerDeath(BasePlayer player, HitInfo info)
        {
			string steamID = player.UserIDString; // Retrieving the SteamID as a string
            if (player != null && !player.IsConnected && steamID.Length == 17)
            {
                player.health = 100;
                player.metabolism.bleeding.value = 0;
                player.Teleport(configData.teleportLocation);
				Puts("Death Prevented for : "+player.displayName);
                return false;
            }
            return null;
        }
    }
}
