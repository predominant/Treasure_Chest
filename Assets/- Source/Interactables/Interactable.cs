using UnityEngine;
using System.Collections;

public interface Interactable
{
	Transform InteractLocator { get; }

    void HandleInteraction();
}