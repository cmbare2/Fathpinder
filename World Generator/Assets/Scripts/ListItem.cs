using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListItem : MonoBehaviour {

	public Text name;
	public InputField inputField;
	public GameObject editButton;
	public GameObject saveButton;
	public GameObject cancelButton;
	public GameObject shadow;
	public GameObject reorderButton;

	public bool highlighted = false;

	private void Update() {
		if (transform.GetSiblingIndex () - 1 == SceneController.characterIdx && !highlighted) {
			highlighted = true;
			GetComponent<Image> ().color = Color.grey;
			GetComponentInChildren<Text> ().color = Color.white;
		} else if (transform.GetSiblingIndex () - 1 != SceneController.characterIdx && highlighted) {
			highlighted = false;
			GetComponent<Image> ().color = Color.white;
			GetComponentInChildren<Text> ().color = Color.black;
		}

		if (transform.parent.childCount < 3 && reorderButton.activeSelf)
			reorderButton.SetActive (false);
		else if (transform.parent.childCount > 2 && !reorderButton.activeSelf)
			reorderButton.SetActive (true);
			
	}

	public void Edit(bool edit) {
		if (edit && !SceneController.editing) {
			SceneController.editing = true;
			name.gameObject.SetActive (false);
			inputField.gameObject.SetActive (true);
			inputField.text = name.text;
			editButton.SetActive (false);
			saveButton.SetActive (true);
			cancelButton.SetActive (true);
		} else {
			SceneController.editing = false;
			name.gameObject.SetActive (true);
			inputField.gameObject.SetActive (false);
			editButton.SetActive (true);
			saveButton.SetActive (false);
			cancelButton.SetActive (false);
		}
	}

	public void Save() {
		name.text = inputField.text;
		Edit (false);
	}
}
