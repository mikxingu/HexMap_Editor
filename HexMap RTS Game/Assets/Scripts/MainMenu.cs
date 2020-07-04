using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour
{
	public Sprite[] backgroundImages;

	public Image backgroundImage;

	public void Quit()
	{
		Debug.Log("User quit the game.");
		Application.Quit();
	}

	void OnEnable()
	{
		StartCoroutine("ChangeBackground");
		backgroundImage.enabled = true;
	}

	public IEnumerator ChangeBackground()
	{
		while (true)
		{
			//Fade In
			backgroundImage.sprite = backgroundImages[Random.Range(0, backgroundImages.Length)];
			backgroundImage.enabled = true;
			yield return new WaitForSeconds(5);
			//Fade Out
			backgroundImage.enabled = false;
			yield return new WaitForSeconds(1);
		}
	}

	public void GoToLink()
	{
		Application.OpenURL("https://github.com/mikxingu/HexMap_Editor");
	}

	public void GoToEditor()
	{
		SceneManager.LoadScene("Editor");
	}
}
