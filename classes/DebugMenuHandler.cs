using Core;
using UnityEngine;

public class DebugMenuHandler : MonoBehaviour
{
	public void FixedUpdate()
	{
        DebugMenuB.MakeDebugMenuExist(); // Moved outside to display mod indicator
        if (GameController.FreezeFixedUpdate || (!CheatsHandler.Instance.DebugEnabled && !CheatsHandler.DebugAlwaysEnabled) || !Core.Input.RightStick.OnPressed)
		{
			return;
		}
		if (Core.Input.Grab.Pressed)
		{
			if ((bool)DebugMenuB.Instance)
			{
				HierarchyDebugMenu component = DebugMenuB.Instance.GetComponent<HierarchyDebugMenu>();
				if ((bool)component)
				{
					component.enabled = true;
				}
			}
		}
		else
		{
			DebugMenuB.ToggleDebugMenu();
		}
	}
}
