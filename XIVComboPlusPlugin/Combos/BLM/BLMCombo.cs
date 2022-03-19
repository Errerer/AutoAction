﻿using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVComboPlus.Combos.BLM
{
    internal abstract class BLMCombo : CustomCombo
    {
        public sealed override string JobName => "黑魔法师";

        internal sealed override uint JobID => 25;

        private BLMGauge _gauge;

        public BLMGauge JobGauge
        {
            get
            {
                if( _gauge == null)
                {
                    _gauge = Service.JobGauges.Get<BLMGauge>();
                }
                return _gauge;
            }
        }


        protected sealed override JobGaugeBase GetJobGaugeBase()
        {
            return Service.JobGauges.Get<BLMGauge>();
        }

        public enum Buffs : ushort
        {
            雷云 = 164,

            黑魔纹 = 737,

            三连咏唱 = 1211,

            火苗 = 165,
        }

        public enum Debuffs : ushort
        {
            Thunder = 161,

            Thunder3 = 163,
        }

        public enum Levels : byte
        {
            Fire3 = 35,

            Blizzard3 = 35,

            Freeze = 40,

            Thunder3 = 45,

            Flare = 50,

            Blizzard4 = 58,

            Fire4 = 60,

            BetweenTheLines = 62,

            Despair = 72,

            UmbralSoul = 76,

            Xenoglossy = 80,

            HighFire2 = 82,

            HighBlizzard2 = 82,

            Paradox = 90,
        }
        public enum Actions : uint
        {
            Fire = 141u,

            Blizzard = 142u,

            Thunder = 144u,

            Blizzard2 = 146u,

            Transpose = 149u,

            Fire3 = 152u,

            Thunder3 = 153u,

            Blizzard3 = 154u,

            Scathe = 156u,

            Freeze = 159u,

            Flare = 162u,

            LeyLines = 3573u,

            Blizzard4 = 3576u,

            Fire4 = 3577u,

            BetweenTheLines = 7419u,

            Despair = 16505u,

            UmbralSoul = 16506u,

            Xenoglossy = 16507u,

            Paradox = 25797u,
        }
    }
}
