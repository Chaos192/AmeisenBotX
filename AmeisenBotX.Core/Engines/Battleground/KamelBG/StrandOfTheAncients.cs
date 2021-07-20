﻿using AmeisenBotX.Common.Math;
using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Data.Objects;
using AmeisenBotX.Core.Engines.Battleground.KamelBG.Enums;
using AmeisenBotX.Core.Engines.Movement.Enums;
using AmeisenBotX.Core.Fsm;
using AmeisenBotX.Core.Fsm.States;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Battleground.KamelBG
{
    internal class StrandOfTheAncients : IBattlegroundEngine
    {
        public StrandOfTheAncients(AmeisenBotInterfaces bot, AmeisenBotFsm stateMachine)
        {
            Bot = bot;
            StateMachine = stateMachine;

            CombatEvent = new(TimeSpan.FromSeconds(2));
        }

        public string Author => "Lukas";

        public AmeisenBotInterfaces Bot { get; }

        public string Description => "Strand of the Ancients";

        public string Name => "Strand of the Ancients";

        public List<Vector3> PathRight { get; } = new()
        {
            new(1403, 69, 30)
        };

        private TimegatedEvent CombatEvent { get; }

        private AmeisenBotFsm StateMachine { get; }

        public void Combat()
        {
            IWowPlayer weakestPlayer = Bot.GetNearEnemies<IWowPlayer>(Bot.Player.Position, 30.0f).OrderBy(e => e.Health).FirstOrDefault();

            if (weakestPlayer != null)
            {
                double distance = weakestPlayer.Position.GetDistance(Bot.Player.Position);
                double threshold = Bot.CombatClass.IsMelee ? 3.0 : 28.0;

                if (distance > threshold)
                {
                    Bot.Movement.SetMovementAction(MovementAction.Move, weakestPlayer.Position);
                }
                else if (CombatEvent.Run())
                {
                    StateMachine.GetState<StateCombat>().Mode = CombatMode.Force;
                    Bot.Wow.WowTargetGuid(weakestPlayer.Guid);
                }
            }
            else
            {
            }
        }

        public void Enter()
        {
        }

        public void Execute()
        {
            Combat();

            if (Bot.Objects.Vehicle == null)
            {
                IWowGameobject VehicleNode = Bot.Objects.WowObjects
                    .OfType<IWowGameobject>()
                    .Where(x => Enum.IsDefined(typeof(Vehicle), x.DisplayId)
                            && x.Position.GetDistance(Bot.Player.Position) < 20)
                    .OrderBy(x => x.Position.GetDistance(Bot.Player.Position))
                    .FirstOrDefault();

                if (VehicleNode != null)
                {
                    Bot.Movement.SetMovementAction(MovementAction.Move, VehicleNode.Position);

                    if (Bot.Player.Position.GetDistance(VehicleNode.Position) <= 4)
                    {
                        Bot.Movement.StopMovement();

                        Bot.Wow.WowObjectRightClick(VehicleNode.BaseAddress);
                    }
                }
            }
            else
            {
                Vector3 currentNode = PathRight[0];
                Bot.Movement.SetMovementAction(MovementAction.Move, currentNode);
            }
        }

        public void Leave()
        {
        }
    }
}