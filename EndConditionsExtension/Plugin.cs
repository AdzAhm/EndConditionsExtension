using LabApi.Loader.Features.Plugins;
using LabApi.Loader.Features.Plugins.Enums;
using System;
using UncomplicatedCustomRoles.API.Features;
using ServerEvents = LabApi.Events.Handlers.ServerEvents;
using PlayerEvents = LabApi.Events.Handlers.PlayerEvents;

namespace EndConditionsExtension
{
    public class Plugin : Plugin<Config>
    {
        public override string Name => "EndConditionsExtension";
        public override string Author => "FoxWorn3365, Refactored";
        public override string Description => "Extension for UncomplicatedCustomRoles to add custom end conditions.";
        public override Version Version => new(1, 0, 0);
        public override Version RequiredApiVersion => new(1, 1, 7);
        public override LoadPriority Priority => LoadPriority.Medium;

        public static Plugin Instance;
        internal Handler Handler;

        public override void Enable()
        {
            Instance = this;
            Handler = new();

            // UCR Event Registration
            UcrEvents.CustomRoleEscaped += Handler.OnCustomRoleEscaped;

            // LabAPI Event Registration
            ServerEvents.RoundEnding += Handler.OnRoundEnding;
            ServerEvents.WaitingForPlayers += Handler.OnWaitingForPlayers;
        }

        public override void Disable()
        {
            // LabAPI Event Unregistration
            ServerEvents.RoundEnding -= Handler.OnRoundEnding;
            ServerEvents.WaitingForPlayers -= Handler.OnWaitingForPlayers;

            // UCR Event Unregistration
            UcrEvents.CustomRoleEscaped -= Handler.OnCustomRoleEscaped;

            Instance = null;
            Handler = null;
        }
    }
}
