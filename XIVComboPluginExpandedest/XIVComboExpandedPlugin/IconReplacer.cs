using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Hooking;
using Dalamud.Logging;
using Lumina.Excel.GeneratedSheets;
using XIVComboExpandedPlugin.Combos;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace XIVComboExpandedPlugin;

internal sealed class IconReplacer : IDisposable
{
	private delegate ulong IsIconReplaceableDelegate(uint actionID);

	private delegate uint GetIconDelegate(IntPtr actionManager, uint actionID);

	private delegate IntPtr GetActionCooldownSlotDelegate(IntPtr actionManager, int cooldownGroup);

	[StructLayout(LayoutKind.Explicit)]
	internal struct CooldownData
	{
		[FieldOffset(0)]
		public bool IsCooldown;

		[FieldOffset(4)]
		public uint ActionID;

		[FieldOffset(8)]
		public float CooldownElapsed;

		[FieldOffset(12)]
		public float CooldownTotal;

		public float CooldownRemaining
		{
			get
			{
				if (!IsCooldown)
				{
					return 0f;
				}
				return CooldownTotal - CooldownElapsed;
			}
		}
	}

	private readonly List<CustomCombo> customCombos;

	private readonly Hook<IsIconReplaceableDelegate> isIconReplaceableHook;

	private readonly Hook<GetIconDelegate> getIconHook;

	private IntPtr actionManager = IntPtr.Zero;

	private HashSet<uint> comboActionIDs = new HashSet<uint>();

	private readonly Dictionary<uint, byte> cooldownGroupCache = new Dictionary<uint, byte>();

	private readonly GetActionCooldownSlotDelegate getActionCooldownSlot;

	public IconReplacer()
	{
		customCombos = (from t in Assembly.GetAssembly(typeof(CustomCombo))!.GetTypes()
			where t.BaseType == typeof(CustomCombo)
			select Activator.CreateInstance(t)).Cast<CustomCombo>().ToList();
		UpdateEnabledActionIDs();
		getActionCooldownSlot = Marshal.GetDelegateForFunctionPointer<GetActionCooldownSlotDelegate>(Service.Address.GetActionCooldown);
		getIconHook = new Hook<GetIconDelegate>(Service.Address.GetAdjustedActionId, (GetIconDelegate)GetIconDetour);
		isIconReplaceableHook = new Hook<IsIconReplaceableDelegate>(Service.Address.IsActionIdReplaceable, (IsIconReplaceableDelegate)IsIconReplaceableDetour);
		getIconHook.Enable();
		isIconReplaceableHook.Enable();
	}

	public void Dispose()
	{
		getIconHook.Dispose();
		isIconReplaceableHook.Dispose();
	}

	internal void UpdateEnabledActionIDs()
	{
		comboActionIDs = customCombos.Where((CustomCombo combo) => Service.Configuration.EnabledActions.Contains(combo.Preset)).SelectMany((CustomCombo combo) => combo.ActionIDs).ToHashSet();
	}

	internal uint OriginalHook(uint actionID)
	{
		return getIconHook.Original.Invoke(actionManager, actionID);
	}

	private unsafe uint GetIconDetour(IntPtr actionManager, uint actionID)
	{
		this.actionManager = actionManager;
		try
		{
			PlayerCharacter localPlayer = Service.ClientState.LocalPlayer;
			if ((GameObject)(object)localPlayer == (GameObject)null || !comboActionIDs.Contains(actionID))
			{
				return OriginalHook(actionID);
			}
			uint lastComboActionID = *(uint*)(void*)Service.Address.LastComboMove;
			float comboTime = *(float*)(void*)Service.Address.ComboTimer;
			byte level = ((Character)localPlayer).Level;
			foreach (CustomCombo customCombo in customCombos)
			{
				if (customCombo.TryInvoke(actionID, lastComboActionID, comboTime, level, out var newActionID))
				{
					return newActionID;
				}
			}
			return OriginalHook(actionID);
		}
		catch (Exception ex)
		{
			PluginLog.Error(ex, "Don't crash the game", Array.Empty<object>());
			return OriginalHook(actionID);
		}
	}

	private ulong IsIconReplaceableDetour(uint actionID)
	{
		return 1uL;
	}

	internal CooldownData GetCooldown(uint actionID)
	{
		byte cooldownGroup = GetCooldownGroup(actionID);
		if (actionManager == IntPtr.Zero)
		{
			CooldownData result = default(CooldownData);
			result.ActionID = actionID;
			return result;
		}
		return Marshal.PtrToStructure<CooldownData>(getActionCooldownSlot(actionManager, cooldownGroup - 1));
	}

	private byte GetCooldownGroup(uint actionID)
	{
		if (cooldownGroupCache.TryGetValue(actionID, out var value))
		{
			return value;
		}
		Action row = Service.DataManager.GetExcelSheet<Action>().GetRow(actionID);
		return cooldownGroupCache[actionID] = row.CooldownGroup;
	}
}