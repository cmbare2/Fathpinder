using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MoveTo : MonoBehaviour
{
	public Vector3 origin { get; set; }
	public bool bIsAnimating { get; set; }
	public bool bRubber = false;

	void Awake()
	{
		bIsAnimating = false;
		origin = transform.localPosition;
	}

	public void GoHome(float speed)
	{
		if (transform.localPosition == origin) return;
		bIsAnimating = true;
		StopAllCoroutines();
		StartCoroutine(MoveToPosCR(origin, speed));
	}

	public void MoveImmediate(Vector3 target)
	{
		StopAllCoroutines();
		bIsAnimating = false;
		transform.localPosition = target;
	}

	public void MoveToPos(Vector3 target, float speed)
	{
		if (transform.localPosition == target) return;
		bIsAnimating = true;
		StopAllCoroutines();
		StartCoroutine(MoveToPosCR(target, speed));
	}

	private IEnumerator MoveToPosCR(Vector3 target, float speed)
	{
		Vector3 current = transform.localPosition;
		float fPhase = 0f;

		if (bRubber)
		{
			Vector3 difference = target - current;
			difference *= 1.15f;
			Vector3 bandedTarget = current + difference;

			while (fPhase < 1f)
			{
				fPhase = Mathf.Clamp01(fPhase + Time.deltaTime * speed);
				float easedF = iTween.easeOutQuad(0f, 1f, fPhase);
				transform.localPosition = Vector3.Lerp(current, bandedTarget, easedF);
				yield return null;
			}
			current = transform.localPosition;
			fPhase = 0f;
			while (fPhase < 1f)
			{
				fPhase = Mathf.Clamp01(fPhase + Time.deltaTime * speed * 2);
				float easedF = iTween.easeOutQuad(0f, 1f, fPhase);
				transform.localPosition = Vector3.Lerp(current, target, easedF);
				yield return null;
			}
			bIsAnimating = false;
		}
		else
		{
			while (fPhase < 1f)
			{
				fPhase = Mathf.Clamp01(fPhase + Time.deltaTime * speed);
				float easedF = iTween.easeOutQuad(0f, 1f, fPhase);
				transform.localPosition = Vector3.Lerp(current, target, easedF);
				yield return null;
			}
			bIsAnimating = false;
		}
	}
}

