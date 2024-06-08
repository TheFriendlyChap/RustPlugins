using System;
using Newtonsoft.Json;
/*	MIT License

	©2024 The Friendly Chap

	Permission is hereby granted, free of charge, to any person obtaining a copy
	of this software and associated documentation files (the "Software"), to deal
	in the Software without restriction, including without limitation the rights
	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	copies of the Software, and to permit persons to whom the Software is
	furnished to do so, subject to the following conditions:

	The above copyright notice and this permission notice shall be included in all
	copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
	SOFTWARE.
*/	
namespace Oxide.Plugins
{
    [Info("AlwaysWetPlanters", "The Friendly Chap", "1.0.0")]
    [Description("Ensures planters on the map without an owner always show as 100% wet")]
    class AlwaysWetPlanters : RustPlugin
    {
		
        private void Init()
        {
            AdjustAllPlanterBoxes();
            timer.Every(300f, () => AdjustAllPlanterBoxes());
        }
        private void OnGrowableStateChange(GrowableEntity entity, PlantProperties.State state)
        {
            AdjustSoilSaturation(entity);
        }
        private void OnEntitySpawned(BaseEntity entity)
        {
            if (entity is GrowableEntity growableEntity)
            {
                AdjustSoilSaturation(growableEntity);
            }
        }
        private void AdjustAllPlanterBoxes()
        {
            foreach (var entity in BaseEntity.serverEntities)
            {
                if (entity is PlanterBox planterBox)
                {
                    AdjustPlanterSoilSaturation(planterBox);
                }
            }
        }
        private void AdjustSoilSaturation(GrowableEntity entity)
        { 
            PlanterBox planterBox = entity.GetPlanter();
            if (planterBox == null) return; 
            AdjustPlanterSoilSaturation(planterBox);
        }
        private void AdjustPlanterSoilSaturation(PlanterBox planterBox)
        {
            if (!(_config.Owned) )
            {
                if (planterBox.OwnerID != 0) return;
            }
            planterBox.soilSaturation = planterBox.soilSaturationMax;
            planterBox.ForceUpdateTriggers();
        }
        private Configuration _config;
        private class Configuration
        {
            [JsonProperty("Extend to Players?")]
            public bool Owned = false; 
        }
        protected override void LoadConfig()
        {
            base.LoadConfig();
            try
            {
                _config = Config.ReadObject<Configuration>();
                if (_config == null) throw new Exception();
                SaveConfig(); 
            }
            catch
            {
                PrintError("Failed to load config, using default values");
                LoadDefaultConfig(); 
            }
        }
        protected override void LoadDefaultConfig() => _config = new Configuration();
        protected override void SaveConfig() => Config.WriteObject(_config);
    }
}