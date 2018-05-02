using UnityEngine;
using System.Collections;

public class GameUtility : MonoBehaviour {

	public static SpriteRenderer GetSpriteRendererRecursively(Transform root) {
		SpriteRenderer spriteRend = root.GetComponent<SpriteRenderer>();

		foreach (Transform element in root) {
			spriteRend = element.GetComponent<SpriteRenderer>();
			if (spriteRend != null)
				return spriteRend;

			GetSpriteRendererRecursively(element);
		}

		if (spriteRend != null)
			return spriteRend;

		return null;
	}

	public static void ChangeLayerRecursively(Transform root, int layer) {
		foreach (Transform element in root) {
			if (element.gameObject.layer != layer)
				element.gameObject.layer = layer;

			ChangeLayerRecursively(element, layer);
		}

		if (root.gameObject.layer != layer)
			root.gameObject.layer = layer;
	}

	public static void ChangeSortingLayerRecursively(Transform root, string layer) {
		Renderer rend = root.GetComponent<Renderer>();

		foreach (Transform element in root) {
			rend = element.GetComponent<Renderer>();
			if (rend != null)
				rend.sortingLayerName = layer;

			ChangeSortingLayerRecursively(element, layer);
		}

		if (rend != null)
			rend.sortingLayerName = layer;
	}

	public static void ChangeSortingLayerRecursively(Transform root, string layer, string layerExemption) {
		Renderer rend = root.GetComponent<Renderer>();

		foreach (Transform element in root) {
			rend = element.GetComponent<Renderer>();
			if (rend != null && rend.sortingLayerName != layerExemption)
				rend.sortingLayerName = layer;

			ChangeSortingLayerRecursively(element, layer, layerExemption);
		}

		if (rend != null && rend.sortingLayerName != layerExemption)
			rend.sortingLayerName = layer;
	}

	public static BaseCharacterEntity SearchBaseCharacterEntity(Transform root) {
		BaseCharacterEntity characterEntity = null;
		foreach (Transform element in root) {
			characterEntity = element.GetComponent<BaseCharacterEntity>();
			if (characterEntity != null)
				return characterEntity;

			SearchBaseCharacterEntity(element);
		}

		characterEntity = root.GetComponent<BaseCharacterEntity>();
		if (characterEntity != null)
			return characterEntity;

		return characterEntity;
	}

	public static Vector3 GetWorldScale(Transform trans) {
		Vector3 worldScale = trans.localScale;
		Transform parent = trans.parent;

		while (parent != null) {
			worldScale = Vector3.Scale(worldScale, parent.localScale);
			parent = parent.parent;
		}

		return worldScale;
	}

	public static void SetRendererEnableRecursively(Transform root, bool active) {
		Renderer rend = root.GetComponent<Renderer>();
		if (rend != null && rend.enabled != active)
			rend.enabled = active;

		foreach (Transform element in root) {
			rend = element.GetComponent<Renderer>();
			if (rend != null && rend.enabled != active)
				element.GetComponent<Renderer>().enabled = active;

			SetRendererEnableRecursively(element, active);
		}
	}
}
