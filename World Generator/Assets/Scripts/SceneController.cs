using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using cakeslice;

public class SceneController : Singleton<SceneController>
{
	public static bool editing = false;
	public static int characterIdx = -1;
	public static int newCharacterIdx = 0;
	public Camera topDownCamera;
	public Transform perspectiveCamera;
	public GameObject grid;
	public GameObject characterCamera;
	public Transform newCharacterHolder;
	public Transform characterHolder;
	public GameObject fullMapCamera;
	public GameObject player;
	public MoveTo characterMenu;
	public Transform chevron;
	public GameObject characterMessage;
	public GameObject originalListItem;
	public Transform canvas;

	private bool topDown = true;
	private GameObject holder;
	private bool newCharacter = false;
	private GameObject mimic = null;
	private Vector2 offset;

	void Start() {
		characterMenu.MoveImmediate(new Vector3(characterMenu.transform.localPosition.x - 300f, characterMenu.transform.localPosition.y));

		var hit = new RaycastHit ();
		if (Physics.Raycast (topDownCamera.transform.position, -Vector3.up, out hit)) {
			perspectiveCamera.LookAt (hit.point);
		}

		holder = topDownCamera.transform.parent.gameObject;

		Renderer[] renderers = newCharacterHolder.GetComponentsInChildren<Renderer> ();
		foreach (Renderer renderer in renderers) {
			cakeslice.Outline outline = renderer.gameObject.AddComponent<cakeslice.Outline> ();
			outline.color = 2;
			outline.enabled = false;
		}

		foreach (Transform character in newCharacterHolder) {
			if (character.GetSiblingIndex () != 0)
				character.gameObject.SetActive (false);
		}
	}

	void Update() {
		if (mimic != null) {
			mimic.transform.position = new Vector2 (Input.mousePosition.x + offset.x, Input.mousePosition.y + offset.y);

			Transform reordering = null;
			foreach (Transform listItem in originalListItem.transform.parent) {
				if (listItem.GetComponent<CanvasGroup> ().alpha == 0f) {
					reordering = listItem;
					break;
				}
			}

			if (reordering.GetSiblingIndex () > 1) { // If not first, check if the mimic is higher than the list item above
				if (originalListItem.transform.parent.GetChild (reordering.GetSiblingIndex () - 1).position.y < mimic.transform.position.y) {
					if (characterIdx == reordering.GetSiblingIndex () - 2)
						characterIdx++;
					else if (characterIdx == reordering.GetSiblingIndex () - 1)
						characterIdx--;
					
					characterHolder.GetChild (reordering.GetSiblingIndex () - 1).SetSiblingIndex (reordering.GetSiblingIndex () - 2);
					reordering.SetSiblingIndex (reordering.GetSiblingIndex() - 1);
				}
			}
			if (reordering.GetSiblingIndex () < originalListItem.transform.parent.childCount - 1) { // If not last, check if the mimic is lower than the list item below
				if (originalListItem.transform.parent.GetChild (reordering.GetSiblingIndex () + 1).position.y > mimic.transform.position.y) {
					if (characterIdx == reordering.GetSiblingIndex ())
						characterIdx--;
					else if (characterIdx == reordering.GetSiblingIndex () - 1)
						characterIdx++;

					characterHolder.GetChild (reordering.GetSiblingIndex () - 1).SetSiblingIndex (reordering.GetSiblingIndex ());
					reordering.SetSiblingIndex (reordering.GetSiblingIndex() + 1);
				}
			}
		}

		if (!editing) {
			if (newCharacter) {
				if (Input.GetKeyDown (KeyCode.Escape))
					ExitNewCharacter ();

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
					AddNewCharacter ();
				}
			} else {
				var hit = new RaycastHit ();

				if (Input.GetMouseButtonDown (0)) {
					Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
					if (Physics.Raycast (ray, out hit)) {
						if (hit.collider.tag == "Player") {
							characterIdx = hit.transform.GetSiblingIndex ();
						}
					}
				}

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
						} else if (Input.GetKeyDown (KeyCode.Escape)) {
							characterIdx = -1;
							if (characterMenu.transform.localPosition.x == characterMenu.origin.x)
								ToggleCharacterMenu ();
						} else if (Input.GetKeyDown (KeyCode.Delete) || Input.GetKeyDown (KeyCode.Backspace)) {
							RemoveCharacter ();
						}
					}
				} else {
					if (player.activeSelf) {
						holder.transform.position = new Vector3 (Mathf.Round (player.transform.position.x / 2f) * 2f, holder.transform.position.y, Mathf.Round (player.transform.position.z / 2f) * 2f);
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
					ShowNewCharacter ();
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
				}
			}
		}
	}

	public void ToggleCharacterMenu() {
		if (characterMenu.transform.localPosition == characterMenu.origin) {
		    characterMenu.MoveToPos(new Vector3(characterMenu.transform.localPosition.x - 300f, characterMenu.transform.localPosition.y), 2f);
			chevron.localEulerAngles = Vector3.zero;
		} else {
			characterMenu.GoHome(2f);
			chevron.localEulerAngles = new Vector3 (chevron.localEulerAngles.x, chevron.localEulerAngles.y, 180f);
		
			if (characterIdx == -1)
				characterIdx = 0;
		}
	}

	public void AddButtonPressed() {
		if (newCharacter) {
			AddNewCharacter ();
		} else {
			ShowNewCharacter ();
		}
	}

	public void RemoveButtonPressed() {
		if (!newCharacter)
			RemoveCharacter ();
	}

	public void StartReorder(GameObject listItem) {
		mimic = Instantiate (listItem);
		mimic.GetComponent<ListItem> ().shadow.SetActive (true);
		Destroy (mimic.GetComponent<ListItem>());
		mimic.GetComponent<RectTransform> ().sizeDelta = new Vector2 (listItem.GetComponent<RectTransform> ().rect.width, listItem.GetComponent<RectTransform> ().rect.height);
		mimic.transform.SetParent (canvas);
		offset = new Vector2(listItem.transform.position.x - Input.mousePosition.x, listItem.transform.position.y - Input.mousePosition.y);

		// Hide listItem, but save spot
		listItem.GetComponent<CanvasGroup>().alpha = 0f;
	}

	public void EndReorder() {
		Destroy (mimic);

		// Show listItem
		foreach (CanvasGroup canvasGroup in originalListItem.transform.parent.GetComponentsInChildren<CanvasGroup>()) {
			canvasGroup.alpha = 1f;
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

	private void AddNewCharacter() {
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
		newCharacter.GetComponent<Character> ().added = true;

		characterIdx = characterHolder.childCount - 1;

		// Add list item to menu
		GameObject newListItem = Instantiate(originalListItem);
		newListItem.transform.SetParent (originalListItem.transform.parent);
		newListItem.SetActive (true);
		newListItem.GetComponentInChildren<Text> ().text = newCharacterHolder.GetChild (newCharacterIdx).gameObject.name;
		newListItem.GetComponent<Button> ().onClick.AddListener (new UnityEngine.Events.UnityAction(delegate {
			characterIdx = newListItem.transform.GetSiblingIndex () - 1;
		}));
		characterMessage.SetActive (false);
	}

	private void ShowNewCharacter() {
		if (topDown)
			topDownCamera.gameObject.SetActive (false);
		else
			perspectiveCamera.gameObject.SetActive (false);
		newCharacter = true;
		characterCamera.SetActive (true);
	}

	private void RemoveCharacter() {
		Destroy (characterHolder.GetChild (characterIdx).gameObject);
		Destroy (originalListItem.transform.parent.GetChild (characterIdx + 1).gameObject);
		--characterIdx;
		if (characterIdx == -1) {
			characterMessage.SetActive (true);
			if (characterMenu.transform.localPosition.x == characterMenu.origin.x)
				ToggleCharacterMenu ();
		}
	}
}
