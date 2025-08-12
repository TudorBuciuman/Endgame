using UnityEngine;

public class OverworldManipulator : MonoBehaviour
{
	[SerializeField]
	private bool hide = true;

	protected virtual void Awake()
	{
		if (hide)
		{
			base.transform.GetComponent<SpriteRenderer>().enabled = false;
		}
	}
}
