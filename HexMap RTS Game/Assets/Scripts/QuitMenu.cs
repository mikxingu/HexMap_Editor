using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitMenu : MonoBehaviour
{
	public void Open()
	{
		gameObject.SetActive(true);
		HexMapCamera.Locked = true;
	}

	public void Close()
	{
		gameObject.SetActive(false);
		HexMapCamera.Locked = false;
	}

	public void Quit()
	{
		Debug.Log("User quit the game.");
		Application.Quit();
	}
}
