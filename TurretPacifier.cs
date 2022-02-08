namespace Oxide.Plugins
{
    [Info("Turret Pacifier", "The Friendly Chap", "1.0.0")]
    [Description("Prevents AutoTurret from targeting Non-Hostile players.")]
    class TurretPacifier : RustPlugin
    {
        private object CanBeTargeted(BasePlayer player, AutoTurret turret)
        {
            return !player.IsNpc && !player.IsHostile() ? false : (object)null;
        }
    }
}