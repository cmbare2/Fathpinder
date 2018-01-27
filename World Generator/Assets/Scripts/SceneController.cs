using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using cakeslice;

public class SceneController : MonoBehaviour
{
	public Camera topDownCamera;
	public Transform perspectiveCamera;
	public GameObject grid;
	public GameObject characterCamera;
	public Transform newCharacterHolder;
	public Transform characterHolder;
	public GameObject fullMapCamera;
	public GameObject player;

	private bool topDown = true;
	private GameObject holder;
	private bool newCharacter = false;
	private int newCharacterIdx = 0;
	private int characterIdx = -1;

	void Start() {
		var hit = new RaycastHit ();
		if (Physics.Raycast (topDownCamera.transform.position, -Vector3.up, out hit)) {
			perspectiveCamera.LookAt (hit.point);
		}

		holder = topDownCamera.transform.parent.gameObject;

		Renderer[] renderers = newCharacterHolder.GetComponentsInChildren<Renderer> ();
		foreach (Renderer renderer in renderers) {
			Outline outline = renderer.gameObject.AddComponent<Outline> ();
			outline.color = 2;
			outline.enabled = false;
		}

		foreach (Transform character in newCharacterHolder) {
			if (character.GetSiblingIndex () != 0)
				character.gameObject.SetActive (false);
		}
	}

	void Update() {
		if (newCharacter) {
			if (Input.GetKeyDown (KeyCode.Escape)) ExitNewCharacter ();

			if (Input.GetKeyDown (KeyCode.A)) {
				newCharacterHolder.GetChild (newCharacterIdx).gameObject.SetActive (false);
				--newCharacterIdx;
				if (newCharacterIdx == -1)
					newCharacterIdx = newCharacterHolder.childCount - 1;
				newCharacterHolder.GetChild (newCharacterIdx).gameObject.SetActive (true);
			} else if (Input.GetKeyDown (KeyCode.D)) {
				newCharacterHolder.GetChild (newCharacterIdx).gameObject.SetActive (false);
				++newCharacterIdx;
				if (newCharacterIdx == newCharacterHolder.childCount)
					newCharacterIdx = 0;
				newCharacterHolder.GetChild (newCharacterIdx).gameObject.SetActive (true);
			} else if (Input.GetKeyDown (KeyCode.Return)) {
				ExitNewCharacter ();
				GameObject newCharacter = Instantiate (newCharacterHolder.GetChild (newCharacterIdx).gameObject);
				newCharacter.transform.SetParent (characterHolder);
				var hit = new RaycastHit ();
				if (Physics.Raycast (topDownCamera.transform.position, -Vector3.up, out hit)) {
					newCharacter.transform.position = new Vector3 (hit.point.x - 1, hit.point.y, hit.point.z + 1);
					if (Physics.Raycast (newCharacter.transform.position, -Vector3.up, out hit)) {
						newCharacter.transform.position = hit.point;
					} else if (Physics.Raycast (newCharacter.transform.position, Vector3.up, out hit)) {
						newCharacter.transform.position = hit.point;
					}
				}
					
				foreach (Outline outline in characterHolder.GetComponentsInChildren<Outline>()) {
					outline.enabled = false;
				}
				characterIdx = characterHolder.childCount - 1;
				foreach (Outline outline in newCharacter.GetComponentsInChildren<Outline>()) {
					outline.enabled = true;
				}
			}
		} else {
			var hit = new RaycastHit ();
			if (characterIdx != -1) {
				if (Input.GetKeyDown (KeyCode.A) || Input.GetKeyDown (KeyCode.D) || Input.GetKeyDown (KeyCode.W) || Input.GetKeyDown (KeyCode.S)) {
					if (Input.GetKeyDown (KeyCode.A)) {
						characterHolder.GetChild (characterIdx).position = new Vector3 (characterHolder.GetChild (characterIdx).position.x - 2, characterHolder.GetChild (characterIdx).position.y, characterHolder.GetChild (characterIdx).position.z);
					} else if (Input.GetKeyDown (KeyCode.D)) {
						characterHolder.GetChild (characterIdx).position = new Vector3 (characterHolder.GetChild (characterIdx).position.x + 2, characterHolder.GetChild (characterIdx).position.y, characterHolder.GetChild (characterIdx).position.z);
					} else if (Input.GetKeyDown (KeyCode.W)) {
						characterHolder.GetChild (characterIdx).position = new Vector3 (characterHolder.GetChild (characterIdx).position.x, characterHolder.GetChild (characterIdx).position.y, characterHolder.GetChild (characterIdx).position.z + 2);
					} else if (Input.GetKeyDown (KeyCode.S)) {
						characterHolder.GetChild (characterIdx).position = new Vector3 (characterHolder.GetChild (characterIdx).position.x, characterHolder.GetChild (characterIdx).position.y, characterHolder.GetChild (characterIdx).position.z - 2);
					}

					if (Physics.Raycast (characterHolder.GetChild (characterIdx).position, -Vector3.up, out hit)) {
						characterHolder.GetChild (characterIdx).position = hit.point;
					} else if (Physics.Raycast (characterHolder.GetChild (characterIdx).position, Vector3.up, out hit)) {
						characterHolder.GetChild (characterIdx).position = hit.point;
					}
				} else {
					if (Input.GetKey (KeyCode.LeftArrow)) {
						characterHolder.GetChild (characterIdx).localEulerAngles = new Vector3 (characterHolder.GetChild (characterIdx).localEulerAngles.x, characterHolder.GetChild (characterIdx).localEulerAngles.y - 5f);
					} else if (Input.GetKey (KeyCode.RightArrow)) {
						characterHolder.GetChild (characterIdx).localEulerAngles = new Vector3 (characterHolder.GetChild (characterIdx).localEulerAngles.x, characterHolder.GetChild (characterIdx).localEulerAngles.y + 5f);
					} else if (Input.GetKeyDown (KeyCode.DownArrow) && characterHolder.GetChild (characterIdx).localEulerAngles.x == 0) {
						characterHolder.GetChild (characterIdx).localEulerAngles = new Vector3 (-90f, characterHolder.GetChild (characterIdx).localEulerAngles.y);
					} else if (Input.GetKeyDown (KeyCode.UpArrow) && characterHolder.GetChild (characterIdx).localEulerAngles.x != 0) {
						characterHolder.GetChild (characterIdx).localEulerAngles = new Vector3 (0f, characterHolder.GetChild (characterIdx).localEulerAngles.y);
					} else if (Input.GetKeyDown (KeyCode.Tab)) {
						++characterIdx;
						if (characterIdx == characterHolder.childCount)
							characterIdx = 0;
						UpdateHighlight ();
					} else if (Input.GetKeyDown (KeyCode.Escape)) {
						characterIdx = -1;
						UpdateHighlight ();
					} else if (Input.GetKeyDown (KeyCode.Delete) || Input.GetKeyDown(KeyCode.Backspace)) {
						Destroy (characterHolder.GetChild (characterIdx).gameObject);
						--characterIdx;
						UpdateHighlight ();
					}
				}
			} else {
				if (player.activeSelf) {
					holder.transform.position = new Vector3 (Mathf.Round(player.transform.position.x / 2f) * 2f, holder.transform.position.y, Mathf.Round(player.transform.position.z / 2f) * 2f);
				} else {
					if (topDown) {
						if (Input.GetKeyDown (KeyCode.Alpha1)) {
							grid.SetActive (true);
							topDownCamera.gameObject.SetActive (true);
							fullMapCamera.SetActive (false);
							topDownCamera.orthographicSize = 10f;
						} else if (Input.GetKeyDown (KeyCode.Alpha2)) {
							grid.SetActive (true);
							topDownCamera.gameObject.SetActive (true);
							fullMapCamera.SetActive (false);
							topDownCamera.orthographicSize = 20f;
						} else if (Input.GetKeyDown (KeyCode.Alpha3)) {
							grid.SetActive (false);
							characterIdx = -1;
							UpdateHighlight ();
							fullMapCamera.SetActive (true);
							topDownCamera.gameObject.SetActive (false);
						} else if (Input.GetKeyDown (KeyCode.A) || (fullMapCamera.activeSelf && Input.GetKey (KeyCode.A))) {
							holder.transform.localPosition = new Vector3 (holder.transform.localPosition.x - 2, holder.transform.localPosition.y, holder.transform.localPosition.z);
						} else if (Input.GetKeyDown (KeyCode.D) || (fullMapCamera.activeSelf && Input.GetKey (KeyCode.D))) {
							holder.transform.localPosition = new Vector3 (holder.transform.localPosition.x + 2, holder.transform.localPosition.y, holder.transform.localPosition.z);
						} else if (Input.GetKeyDown (KeyCode.W) || (fullMapCamera.activeSelf && Input.GetKey (KeyCode.W))) {
							holder.transform.localPosition = new Vector3 (holder.transform.localPosition.x, holder.transform.localPosition.y, holder.transform.localPosition.z + 2);
						} else if (Input.GetKeyDown (KeyCode.S) || (fullMapCamera.activeSelf && Input.GetKey (KeyCode.S))) {
							holder.transform.localPosition = new Vector3 (holder.transform.localPosition.x, holder.transform.localPosition.y, holder.transform.localPosition.z - 2);
						}
					} else if (!player.activeSelf) {
						if (Physics.Raycast (topDownCamera.transform.position, -Vector3.up, out hit)) {
							if (Input.GetKey (KeyCode.A)) {
								perspectiveCamera.RotateAround (hit.point, Vector3.up, 100f * Time.deltaTime);
							} else if (Input.GetKey (KeyCode.D)) {
								perspectiveCamera.RotateAround (hit.point, Vector3.up, -100f * Time.deltaTime);
							} else if (Input.GetKey (KeyCode.W)) {
								if (Vector3.Distance (hit.point, perspectiveCamera.position) > 10f)
									perspectiveCamera.position = Vector3.MoveTowards (perspectiveCamera.position, hit.point, 100f * Time.deltaTime);
							} else if (Input.GetKey (KeyCode.S)) {
								perspectiveCamera.position = Vector3.MoveTowards (perspectiveCamera.position, hit.point, -100f * Time.deltaTime);
							}
						}
					}

					if (Input.GetKeyDown (KeyCode.Tab)) {
						characterIdx = 0;
						UpdateHighlight ();
					}
				}

				if (Input.GetKeyDown (KeyCode.P)) {
					if (player.activeSelf) {
						player.SetActive (false);
						if (topDown)
							topDownCamera.gameObject.SetActive (true);
						else
							perspectiveCamera.gameObject.SetActive (true);
						characterIdx = -1;
						UpdateHighlight ();
					} else {
						player.SetActive (true);
						topDownCamera.gameObject.SetActive (false);
						fullMapCamera.SetActive (false);
						perspectiveCamera.gameObject.SetActive (false);

						if (Physics.Raycast (topDownCamera.transform.position, -Vector3.up, out hit)) {
							player.transform.position = new Vector3 (hit.point.x, hit.point.y + 2f, hit.point.z);
						}
					}
				}
			}

			if (Input.GetKeyDown (KeyCode.G)) {
				grid.SetActive (!grid.activeSelf);
			} else if (Input.GetKeyDown (KeyCode.N)) {
				if (topDown)
					topDownCamera.gameObject.SetActive (false);
				else
					perspectiveCamera.gameObject.SetActive (false);
				newCharacter = true;
				characterCamera.SetActive (true);
			} else if (Input.GetKeyDown (KeyCode.C) && !player.activeSelf) {
				if (topDown) {
					topDown = false;
					topDownCamera.gameObject.SetActive (false);
					fullMapCamera.SetActive (false);
					perspectiveCamera.gameObject.SetActive (true);

					if (Physics.Raycast (topDownCamera.transform.position, -Vector3.up, out hit)) {
						perspectiveCamera.position = new Vector3 (hit.point.x + 30, hit.point.y + 20f, hit.point.z);
						perspectiveCamera.LookAt (hit.point);
					}
				} else {
					topDown = true;
					topDownCamera.gameObject.SetActive (true);
					perspectiveCamera.gameObject.SetActive (false);
				}

				UpdateHighlight ();
			}
		}
	}

	private void ExitNewCharacter() {
		if (topDown)
			topDownCamera.gameObject.SetActive (true);
		else
			perspectiveCamera.gameObject.SetActive (true);
		newCharacter = false;
		characterCamera.SetActive (false);
	}

	private void UpdateHighlight() {
		foreach (Outline outline in characterHolder.GetComponentsInChildren<Outline>()) {
			outline.enabled = false;
		}

		if (characterIdx != -1) {
			foreach (Outline outline in characterHolder.GetChild (characterIdx).GetComponentsInChildren<Outline>()) {
				outline.enabled = true;
			}
		}
	}
}
