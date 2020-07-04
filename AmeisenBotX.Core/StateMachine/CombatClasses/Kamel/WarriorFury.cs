﻿using AmeisenBotX.Core.Character.Comparators;
using AmeisenBotX.Core.Character.Inventory.Enums;
using AmeisenBotX.Core.Character.Talents.Objects;
using AmeisenBotX.Core.Common;
using AmeisenBotX.Core.Data.Enums;
using AmeisenBotX.Core.Data.Objects.WowObject;
using AmeisenBotX.Core.Statemachine.Enums;
using AmeisenBotX.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmeisenBotX.Core.Statemachine.CombatClasses.Kamel
{
    class WarriorFury : BasicKamelClass
    {
        private const string battleStanceSpell = "Battle Stance";
        private const string berserkerStanceSpell = "Berserker Stance";
        private const string berserkerRageSpell = "Berserker Rage";
        private const string bladestormSpell = "Bladestorm";
        private const string bloodthirstSpell = "Bloodthirst";
        private const string chargeSpell = "Charge";
        private const string cleaveSpell = "Cleave";
        private const string commandingShoutSpell = "Commanding Shout";
        private const string disarmSpell = "Disarm";
        private const string executeSpell = "Execute";
        private const string hamstringSpell = "Hamstring";
        private const string heroicStrikeSpell = "Heroic Strike";
        private const string heroicThrowSpell = "Heroic Throw";
        private const string heroicFurySpell = "Heroic Fury";
        private const string interceptSpell = "Intercept";
        private const string intimidatingShoutSpell = "Intimidating Shout";
        private const string rendSpell = "Rend";
        private const string whirlwindSpell = "Whirlwind";
        private const string retaliationSpell = "Retaliation";
        private const string enragedregenerationSpell = "Enraged Regeneration";
        private const string bloodrageSpell = "Bloodrage";
        private const string pummelSpell = "Pummel";
        private const string slamSpell = "Slam";
        private const string recklessnessSpell = "Recklessness";
        private const string defensiveStanceSpell = "Defensive Stance";
        private const string deathWishSpell = "Death Wish";

        Dictionary<string, DateTime> spellCoolDown = new Dictionary<string, DateTime>();

        public WarriorFury(WowInterface wowInterface) : base()
        {
            WowInterface = wowInterface;
            spellCoolDown.Add(interceptSpell, DateTime.Now);
            spellCoolDown.Add(heroicThrowSpell, DateTime.Now);
            spellCoolDown.Add(intimidatingShoutSpell, DateTime.Now);
            spellCoolDown.Add(retaliationSpell, DateTime.Now);
            spellCoolDown.Add(heroicStrikeSpell, DateTime.Now);
            spellCoolDown.Add(executeSpell, DateTime.Now);
            spellCoolDown.Add(enragedregenerationSpell, DateTime.Now);
            spellCoolDown.Add(bloodrageSpell, DateTime.Now);
            spellCoolDown.Add(pummelSpell, DateTime.Now);
            spellCoolDown.Add(bloodthirstSpell, DateTime.Now);
            spellCoolDown.Add(commandingShoutSpell, DateTime.Now);
            spellCoolDown.Add(slamSpell, DateTime.Now);
            spellCoolDown.Add(recklessnessSpell, DateTime.Now);
            spellCoolDown.Add(whirlwindSpell, DateTime.Now);
            spellCoolDown.Add(heroicFurySpell, DateTime.Now);
            spellCoolDown.Add(berserkerRageSpell, DateTime.Now);
            spellCoolDown.Add(disarmSpell, DateTime.Now);
            spellCoolDown.Add(deathWishSpell, DateTime.Now);
            spellCoolDown.Add(rendSpell, DateTime.Now);
        }

        public override string Author => "Kamel";

        public override bool WalkBehindEnemy => false;

        public override WowClass Class => WowClass.Warrior;

        public override Dictionary<string, dynamic> Configureables { get; set; } = new Dictionary<string, dynamic>();

        public override string Description => "Warrior Fury 2.0";

        public override string Displayname => "Warrior Fury Algea";

        public override bool HandlesMovement => false;

        public override bool IsMelee => true;

        public override bool UseAutoAttacks => true;

        public override IWowItemComparator ItemComparator { get; set; } = new BasicStrengthComparator(new List<ArmorType>() { ArmorType.SHIELDS }, new List<WeaponType>() { WeaponType.ONEHANDED_SWORDS, WeaponType.ONEHANDED_MACES, WeaponType.ONEHANDED_AXES });

        public override CombatClassRole Role => CombatClassRole.Dps;

        public override string Version => "2.0";

        public override TalentTree Talents { get; } = new TalentTree()
        {
            Tree1 = new Dictionary<int, Talent>()
            {
                { 1, new Talent(1, 1, 3) },
                { 3, new Talent(1, 3, 2) },
                { 5, new Talent(1, 5, 2) },
                { 6, new Talent(1, 6, 3) },
                { 9, new Talent(1, 9, 2) },
                { 10, new Talent(1, 10, 3) },
                { 11, new Talent(1, 11, 3) },
            },
            Tree2 = new Dictionary<int, Talent>()
            {
                { 1, new Talent(2, 1, 3) },
                { 3, new Talent(2, 3, 5) },
                { 5, new Talent(2, 5, 5) },
                { 6, new Talent(2, 6, 3) },
                { 10, new Talent(2, 10, 5) },
                { 13, new Talent(2, 13, 3) },
                { 14, new Talent(2, 14, 1) },
                { 16, new Talent(2, 16, 1) },
                { 17, new Talent(2, 17, 5) },
                { 18, new Talent(2, 18, 3) },
                { 19, new Talent(2, 19, 1) },
                { 20, new Talent(2, 20, 2) },
                { 22, new Talent(2, 22, 5) },
                { 23, new Talent(2, 23, 1) },
                { 24, new Talent(2, 24, 1) },
                { 25, new Talent(2, 25, 3) },
                { 26, new Talent(2, 26, 5) },
                { 27, new Talent(2, 27, 1) },
            },
            Tree3 = new Dictionary<int, Talent>(),
        };

        public override void ExecuteCC()
        {
            if (!TargetInLineOfSight)
            {
                return;
            }
            StartAttack();
        }

        public override void OutOfCombatExecute()
        {

        }


        private void StartAttack()
        {
            if (WowInterface.ObjectManager.TargetGuid != 0)
            {
                if (WowInterface.HookManager.GetUnitReaction(WowInterface.ObjectManager.Player, WowInterface.ObjectManager.Target) == WowUnitReaction.Friendly)
                {
                    WowInterface.HookManager.ClearTarget();
                    return;
                }

                if (WowInterface.ObjectManager.Player.IsInMeleeRange(WowInterface.ObjectManager.Target))
                {
                    if (!WowInterface.ObjectManager.Player.IsAutoAttacking && AutoAttackEvent.Run())
                    {
                        WowInterface.HookManager.StartAutoAttack(WowInterface.ObjectManager.Target);
                    }
                }
                else//Range
                {
                    double distance = WowInterface.ObjectManager.Player.Position.GetDistance(WowInterface.ObjectManager.Target.Position);

                }
            }
            else if (TargetSelectEvent.Run())
            {
                WowUnit nearTarget = WowInterface.ObjectManager.GetNearEnemies<WowUnit>(WowInterface.ObjectManager.Player.Position, 30)
                    .Where(e => e.IsInCombat) // To Do e.IsTaggedByMe
                    .OrderBy(e => e.Position.GetDistance(WowInterface.ObjectManager.Player.Position))
                    .FirstOrDefault();

                if (nearTarget != null)
                {
                    WowInterface.HookManager.TargetGuid(nearTarget.Guid);
                    // AmeisenLogger.Instance.Log("FuryWarri", $"Target: {nearTarget}");
                }
            }
        }
        private void CustomCastSpell(string spell, string stance = "Berserker Stance")
        {
            if (!WowInterface.ObjectManager.Player.HasBuffByName(stance))
            {
                WowInterface.HookManager.CastSpell(stance);
            }
            else
            {
                WowInterface.HookManager.CastSpell(spell);
            }
        }
    }
}
