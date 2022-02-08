using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Animal Killer", "The Friendly Chap", "1.0.4")]

    class AnimalKiller : RustPlugin
    {
        #region Declarations
        private const string permChap = "animalkiller.admin";

        Dictionary<string, string> AnimalDict = new Dictionary<string, string>
        {
            { "boar", "assets/rust.ai/agents/boar/boar.prefab" },
            { "stag", "assets/rust.ai/agents/stag/stag.prefab" },
            { "wolf", "assets/rust.ai/agents/wolf/wolf.prefab" },
            { "bear", "assets/rust.ai/agents/bear/bear.prefab" },
            { "chicken", "assets/rust.ai/agents/chicken/chicken.prefab" }
        };
        #endregion

        #region Hooks
        void Init()
        {
            permission.RegisterPermission(permChap, this);
        }
        #endregion

        #region Functions
        [ConsoleCommand("ak")]
        void KillAllCommand(ConsoleSystem.Arg arg)
        {
            // Player is null from the ConsoleSystem.Arg - this means the command was ran via console/rcon, etc.
            if (!permission.UserHasPermission(arg.Player().UserIDString, permChap))
            {
                arg.ReplyWith("You don't have the correct permissions to use this command");
                return;
            }
            if (arg == null)
            {
                return;
            }

            if (arg.Args.Length == 0)
            {
                arg.ReplyWith("Please specify an animal : boar, deer, wolf, bear or chicken. (eg. ak boar)");
                return;
            }

            switch (arg.Args[0])
            {
                case "boar":
                    ServerMgr.Instance.StartCoroutine(KillAnimal(AnimalDict["boar"]));
                    break;

                case "deer":
                    ServerMgr.Instance.StartCoroutine(KillAnimal(AnimalDict["stag"]));
                    break;

                case "wolf":
                    ServerMgr.Instance.StartCoroutine(KillAnimal(AnimalDict["wolf"]));
                    break;

                case "bear":
                    ServerMgr.Instance.StartCoroutine(KillAnimal(AnimalDict["bear"]));
                    break;

                case "chicken":
                    ServerMgr.Instance.StartCoroutine(KillAnimal(AnimalDict["chicken"]));
                    break;

                default:
                    arg.ReplyWith($"{arg.Args[0]} is not a valid selection.");
                    break;
            }
        }
        private IEnumerator KillAnimal(string animalDictKey)
        {
            foreach (var animal in BaseNetworkable.serverEntities.OfType<BaseAnimalNPC>())
            {
                if (animal.PrefabName == animalDictKey)
                {
                    Puts($"Killing Blow : {animal.PrefabName}");
                    animal.AdminKill();
                    yield return new WaitWhile(() => !animal.IsDestroyed);
                }
            }
        }
        #endregion
    }
}
