using Core;
using Game;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CheatsHandler : SaveSerialize
{
	public static CheatsHandler Instance;

	public bool DebugEnabled;

	public static bool DebugWasEnabled;

	public static bool DebugAlwaysEnabled;

	public static bool InfiniteDoubleJumps;

	private int m_currentDebugCombinationIndex;

	private List<Cheat> m_cheats = new List<Cheat>();

	private float m_timer;

	public bool CanUseCheats()
	{
		if (!SpecialAbilityZone.IsInside)
		{
			return false;
		}
		return true;
	}

	public bool IsInsideRainbowZone()
	{
		return SpecialAbilityZone.IsInsideRainbowZone;
	}

	public void ChangeCharacterColor()
	{
		if ((bool)Characters.Sein)
		{
			Characters.Sein.PlatformBehaviour.Visuals.SpriteRenderer.material.color = new Color(FixedRandom.Values[0], FixedRandom.Values[1], FixedRandom.Values[2], 0.5f);
		}
	}

	public void MakeDashRainbow()
	{
		SeinDashAttack.RainbowDashActivated = true;
	}

	public void TeleportOri()
	{
		if ((bool)Characters.Sein && (bool)Scenes.Manager && Scenes.Manager.CurrentScene != null && !GameController.Instance.InputLocked)
		{
			Characters.Sein.Position = Scenes.Manager.CurrentScene.PlaceholderPosition;
		}
	}

	public bool CanActivateInfiniteDoubleJumps()
	{
		if ((bool)GameWorld.Instance)
		{
			return GameWorld.Instance.HasCompletedEverything();
		}
		return false;
	}

	public override void Awake()
	{
		Instance = this;
		List<Core.Input.InputButtonProcessor> list = new List<Core.Input.InputButtonProcessor>();
		list.Add(Core.Input.Up);
		list.Add(Core.Input.Up);
		list.Add(Core.Input.Down);
		list.Add(Core.Input.Down);
		list.Add(Core.Input.RightStick);
		List<Core.Input.InputButtonProcessor> list2 = list;
		m_cheats.Add(new Cheat(list2.ToArray(), ActivateDebugMenu, CanUseCheats));
		m_cheats.Add(new Cheat(new Core.Input.InputButtonProcessor[8]
		{
			Core.Input.Up,
			Core.Input.Right,
			Core.Input.Down,
			Core.Input.Left,
			Core.Input.RightStick,
			Core.Input.Up,
			Core.Input.Up,
			Core.Input.Jump
		}, TeleportOri, null));
		m_cheats.Add(new Cheat(new Core.Input.InputButtonProcessor[8]
		{
			Core.Input.Left,
			Core.Input.Up,
			Core.Input.Right,
			Core.Input.Down,
			Core.Input.Up,
			Core.Input.Up,
			Core.Input.Up,
			Core.Input.Jump
		}, ChangeCharacterColor, null));
		m_cheats.Add(new Cheat(new Core.Input.InputButtonProcessor[10]
		{
			Core.Input.Up,
			Core.Input.Up,
			Core.Input.Down,
			Core.Input.Down,
			Core.Input.Left,
			Core.Input.Right,
			Core.Input.Left,
			Core.Input.Right,
			Core.Input.SoulFlame,
			Core.Input.Jump
		}, MakeDashRainbow, IsInsideRainbowZone));
		m_cheats.Add(new Cheat(new Core.Input.InputButtonProcessor[8]
		{
			Core.Input.Jump,
			Core.Input.Up,
			Core.Input.Up,
			Core.Input.Down,
			Core.Input.Down,
			Core.Input.Jump,
			Core.Input.Up,
			Core.Input.Down
		}, delegate
		{
			InfiniteDoubleJumps = true;
		}, CanActivateInfiniteDoubleJumps));
		//if (File.Exists("c:\\temp\\moonDebugPC.txt"))
		//{
			DebugAlwaysEnabled = true; // Always enable debug menu
		//}
		Events.Scheduler.OnGameReset.Add(OnGameReset);
	}

	public override void OnDestroy()
	{
		base.OnDestroy();
		Events.Scheduler.OnGameReset.Remove(OnGameReset);
	}

	private void OnGameReset()
	{
		InfiniteDoubleJumps = false;
		SeinDashAttack.RainbowDashActivated = false;
	}

	private void Update()
	{
		DebugWasEnabled = (DebugWasEnabled || DebugEnabled);
	}

	private void FixedUpdate()
	{
		if (m_timer > 0f)
		{
			m_timer -= Time.fixedDeltaTime;
			if (m_timer <= 0f)
			{
				m_currentDebugCombinationIndex = 0;
			}
		}
		if (!Core.Input.OnAnyButtonPressed)
		{
			return;
		}
		m_timer = 0.6f;
		if (m_currentDebugCombinationIndex == 0)
		{
			foreach (Cheat cheat in m_cheats)
			{
				cheat.Processing = cheat.Combination[0].IsPressed;
			}
		}
		else
		{
			foreach (Cheat cheat2 in m_cheats)
			{
				if (cheat2.Processing)
				{
					cheat2.Processing = cheat2.Combination[m_currentDebugCombinationIndex].IsPressed;
					if (cheat2.Processing && cheat2.Combination.Length - 1 == m_currentDebugCombinationIndex)
					{
						cheat2.Processing = false;
						cheat2.PerformCheat();
					}
				}
			}
			if (!m_cheats.Exists((Cheat a) => a.Processing))
			{
				m_currentDebugCombinationIndex = 0;
			}
		}
		m_currentDebugCombinationIndex++;
	}

	public void ActivateDebugMenu()
	{
		DebugEnabled = true;
		DebugMenuB.DebugControlsEnabled = true;
		DebugMenuB.ToggleDebugMenu();
	}

	public void EnableCheatsEnabled()
	{
		DebugEnabled = true;
	}

	public void DisableCheatsEnabled()
	{
		DebugEnabled = false;
	}

	public override void Serialize(Archive ar)
	{
		ar.Serialize(ref DebugEnabled);
	}
}
