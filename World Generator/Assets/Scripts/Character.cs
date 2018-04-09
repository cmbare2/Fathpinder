using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {

	public bool added = false;
	private bool highlighted = false;
	
	// Update is called once per frame
	void Update () {
		if (added) {
			if (transform.GetSiblingIndex () == SceneController.characterIdx && !highlighted) {
				highlighted = true;

				foreach (cakeslice.Outline outline in transform.parent.GetComponentsInChildren<cakeslice.Outline>()) {
					outline.enabled = false;
				}

				foreach (cakeslice.Outline outline in GetComponentsInChildren<cakeslice.Outline>()) {
					outline.enabled = true;
				}
			} else if (transform.GetSiblingIndex () != SceneController.characterIdx && highlighted) {
				highlighted = false;
			}
		}

	}
}
