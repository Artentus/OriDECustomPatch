using Game;
using UnityEngine;

public class WorldMapLogic : MonoBehaviour, ISuspendable
{
	//private const int PlayerPositionUpdatePollFrequency = 3; // Unused now

	public static WorldMapLogic Instance;

	private int m_fixedUpdateCounter;

	public CageStructureTool MapEnabledArea;

	private static Vector2[] m_samplePositions = new Vector2[5]
	{
		Vector2.zero,
		new Vector2(-2f, 2f),
		new Vector2(2f, 2f),
		new Vector2(-2f, -2f),
		new Vector2(2f, -2f)
	};

	public bool IsSuspended
	{
		get;
		set;
	}

	public void Awake()
	{
		Instance = this;
		SuspensionManager.Register(this);
	}

	public void OnDestroy()
	{
		if (Instance == this)
		{
			Instance = null;
		}
		SuspensionManager.Unregister(this);
	}

	public void FixedUpdate()
	{
		if (IsSuspended)
		{
			return;
		}
		m_fixedUpdateCounter++;
		//if ((m_fixedUpdateCounter % (60 / PlayerPositionUpdatePollFrequency) == 1) && (bool)UI.Cameras.Current.Target) // Avoid check, just update every frame
        if ((bool)UI.Cameras.Current.Target)
		{
			int completionPercentage = GameWorld.Instance.CompletionPercentage;
			Vector3 position = UI.Cameras.Current.Target.position;
			GameWorld.Instance.VisitMapAreasAtPosition(position);
			UpdateCurrentArea();
			if (completionPercentage != GameWorld.Instance.CompletionPercentage)
			{
				Telemetry.CompletionHeroStat.SendData(GameWorld.Instance.CompletionPercentage + "%");
			}
		}
	}

	public void UpdateCurrentArea()
	{
		if (UI.Cameras.Current == null || UI.Cameras.Current.Target == null)
		{
			return;
		}
		World.CurrentArea = null;
		Vector3 position = UI.Cameras.Current.Target.position;
		int num = 0;
		RuntimeGameWorldArea runtimeGameWorldArea;
		while (true)
		{
			if (num < m_samplePositions.Length)
			{
				runtimeGameWorldArea = FindAreaFromPosition(position + (Vector3)m_samplePositions[num]);
				if (runtimeGameWorldArea != null)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		World.CurrentArea = runtimeGameWorldArea;
	}

	public RuntimeGameWorldArea FindAreaFromPosition(Vector3 cameraPosition)
	{
		for (int i = 0; i < GameWorld.Instance.RuntimeAreas.Count; i++)
		{
			RuntimeGameWorldArea runtimeGameWorldArea = GameWorld.Instance.RuntimeAreas[i];
			Vector3 position = runtimeGameWorldArea.Area.BoundaryCage.transform.InverseTransformPoint(cameraPosition);
			CageStructureTool.Face face = runtimeGameWorldArea.Area.BoundaryCage.FindFaceAtPositionFaster(position);
			if (face != null)
			{
				return runtimeGameWorldArea;
			}
		}
		return null;
	}
}
