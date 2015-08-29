using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine.UI
{
	public class UIWindowManager : MonoBehaviour {
	
		protected virtual void Update()
		{
			// Check for escape key press
			if (Input.GetKeyUp(KeyCode.Escape))
			{
				bool EligibleForShow = true;
				
				// Get the windows list
				List<DUIWindow> windows = DUIWindow.GetWindows();
				
				// Loop through the windows and hide if required
				foreach (DUIWindow window in windows)
				{
					// Check if the window has escape key action
					if (window.escapeKeyAction != DUIWindow.EscapeKeyAction.None)
					{
						// Check if the window should be hidden on escape
						if (window.IsOpen && (window.escapeKeyAction == DUIWindow.EscapeKeyAction.Hide || window.escapeKeyAction == DUIWindow.EscapeKeyAction.Toggle || (window.escapeKeyAction == DUIWindow.EscapeKeyAction.HideIfFocused && window.IsFocused)))
						{
							// Hide the window
							window.Hide();
							
							// Dont allow a window to be shown after a window has been closed
							EligibleForShow = false;
						}
					}
				}
				
				// If we didnt hide any windows with this key press check if we should show a window
				if (EligibleForShow)
				{
					// Loop through the windows again and show if required
					foreach (DUIWindow window in windows)
					{
						// Check if the window has escape key action toggle and is not shown
						if (!window.IsOpen && window.escapeKeyAction == DUIWindow.EscapeKeyAction.Toggle)
						{
							// Show the window
							window.Show();
						}
					}
				}
			}
		}
	}
}