﻿using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;
using System.Linq;
using TaleWorlds.Core;

namespace Populations.Components
{
    class PopulationPartyComponent : PartyComponent
    {
        private Settlement _target;
        private string _name;
        public PopulationPartyComponent(Settlement target, string name) : base()
        {
            _target = target;
            _name = name;
        }

        public static MobileParty CreateParty(string id, string origin, Settlement target, string name)
        {
            return MobileParty.CreateParty(id + origin + target.Name.ToString(), new PopulationPartyComponent(target, String.Format(name, origin)), delegate (MobileParty mobileParty)
            {
                mobileParty.SetPartyUsedByQuest(true);
                mobileParty.Party.Visuals.SetMapIconAsDirty();
                mobileParty.SetInititave(0f, 1f, float.MaxValue);
                mobileParty.ShouldJoinPlayerBattles = false;
                mobileParty.Aggressiveness = 0f;
                mobileParty.SetMoveGoToSettlement(target);
            });
        }

        public static MobileParty CreateSlaveCaravan(string id, Settlement origin, Settlement target, string nameTemplate, int slaves)
        {
            MobileParty caravan = CreateParty(id, origin.Name.ToString(), target, nameTemplate);
            caravan.AddPrisoner(CharacterObject.All.FirstOrDefault(x => x.StringId == "vlandian_recruit_new"), slaves);
            caravan.InitializeMobileParty(origin.Culture.EliteCaravanPartyTemplate, origin.GatePosition, 0f, 0f, slaves);
            GiveMounts(ref caravan);
            GiveFood(ref caravan);
            return caravan;
        }

        private static void GiveMounts(ref MobileParty party)
        {
            int lacking = party.Party.NumberOfRegularMembers - party.Party.NumberOfMounts;
            ItemObject horse = Items.All.FirstOrDefault(x => x.StringId == "sumpter_horse");
            party.ItemRoster.AddToCounts(horse, lacking);
        }

        private static void GiveFood(ref MobileParty party)
        {
            int lacking = (party.InventoryCapacity - (int)party.ItemRoster.TotalWeight) / 10;
            ItemObject food = Items.AllTradeGoods.FirstOrDefault(x => x.StringId == "fish");
            party.ItemRoster.AddToCounts(food, lacking);
        }

        public override Hero PartyOwner => HomeSettlement.OwnerClan.Leader;

        public override TextObject Name
        {
            get
            {
                return new TextObject(String.Format(_name, HomeSettlement.Name.ToString()));
            }
        }

        public override Settlement HomeSettlement
        {
            get => _target;
        }
    }
}
