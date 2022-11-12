﻿using Dalamud.Game.ClientState.Objects.Types;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;

namespace XIVAutoAttack.Helpers
{
    public static class ObjectHelper
    {
        private unsafe static BNpcBase GetObjectNPC(this GameObject obj)
        {
            if (obj == null) return null;
            return Service.DataManager.GetExcelSheet<BNpcBase>().GetRow(obj.DataId);
        }

        public static bool HasLocationSide(this GameObject obj)
        {
            if (obj == null) return false;
            return !(obj.GetObjectNPC()?.Unknown10 ?? false);
        }

        public static bool IsBoss(this BattleChara obj)
        {
            if (obj == null) return false;
            return obj.MaxHp >= GetHealthFromMulty(6.5f)
                || !(obj.GetObjectNPC()?.IsTargetLine ?? true);
        }

        public static float GetHealthRatio(this BattleChara b)
        {
            if (b == null) return 0;
            return (float)b.CurrentHp / b.MaxHp;
        }

        /// <summary>
        /// 用于倾泻所有资源来收尾
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool IsDying(this BattleChara b)
        {
            if (b == null) return false;
            return b.CurrentHp <= GetHealthFromMulty(2);
        }

        /// <summary>
        /// 用于倾泻所有资源来收尾
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool CanDot(this BattleChara b)
        {
            if (b == null) return false;
            return b.CurrentHp >= GetHealthFromMulty(0.8f);
        }

        internal static EnemyLocation FindEnemyLocation(this GameObject enemy)
        {
            Vector3 pPosition = enemy.Position;
            float rotation = enemy.Rotation;
            Vector2 faceVec = new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation));

            Vector3 dir = Service.ClientState.LocalPlayer.Position - pPosition;
            Vector2 dirVec = new Vector2(dir.Z, dir.X);

            double angle = Math.Acos(Vector2.Dot(dirVec, faceVec) / dirVec.Length() / faceVec.Length());

            if (angle < Math.PI / 4) return EnemyLocation.Front;
            else if (angle > Math.PI * 3 / 4) return EnemyLocation.Back;
            return EnemyLocation.Side;
        }

        internal unsafe static bool CanAttack(this GameObject actor)
        {
            if (actor == null) return false;
            return ((delegate*<long, IntPtr, long>)Service.Address.CanAttackFunction)(142L, actor.Address) == 1;
        }

        private static uint GetHealthFromMulty(float mult)
        {
            if (Service.ClientState.LocalPlayer == null) return 0;

            var multi = TargetFilter.GetJobCategory(new BattleChara[] { Service.ClientState.LocalPlayer }, Role.防护).Length == 0 ? mult : mult * 1.5f;
            if (TargetUpdater.PartyMembers.Length > 4)
            {
                multi *= 2;
            }
            else if (TargetUpdater.PartyMembers.Length == 1)
            {
                multi = 0.5f;
            }

            return (uint)(multi * Service.ClientState.LocalPlayer.MaxHp);
        }
    }
}