using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class SaveLoadMenu : MonoBehaviour
{
	public RectTransform listContent;

	public SaveLoadItem itemPrefab;

	public Text menuLabel, actionButtonLabel;

	public HexGrid hexGrid;

	bool saveMode;

	public InputField nameInput;

	const int mapFileVersion = 5;

	string GetSelectedPath ()
	{
		string mapName = nameInput.text;
		if (mapName.Length == 0)
		{
			return null;
		}
		return Path.Combine(Application.persistentDataPath, mapName + ".map");
	}

	void FillList ()
	{
		for (int i = 0; i < listContent.childCount; i++)
		{
			Destroy(listContent.GetChild(i).gameObject);
		}
		string[] paths = Directory.GetFiles(Application.persistentDataPath, "*.map");
		Array.Sort(paths);
		for (int i = 0; i < paths.Length; i++)
		{
			SaveLoadItem item = Instantiate(itemPrefab);
			item.menu = this;
			item.MapName = Path.GetFileNameWithoutExtension(paths[i]);
			item.transform.SetParent(listContent, false);

		}
	}

	public void Open(bool saveMode)
	{
		this.saveMode = saveMode;
		if (saveMode)
		{
			menuLabel.text = "Save Map";
			actionButtonLabel.text = "Save";
		}
		else
		{
			menuLabel.text = "Load Map";
			actionButtonLabel.text = "Load";
		}
		FillList();
		gameObject.SetActive(true);
		HexMapCamera.Locked = true;
	}

	public void Close ()
	{
		gameObject.SetActive(false);
		HexMapCamera.Locked = false;
	}

	public void Save(string path)
	{
		//MÉTODO RESPONSAVEL POR SALVAR O MAPA.
		Debug.Log("Saving Map at: " + Application.persistentDataPath);
		//string path = Path.Combine(Application.persistentDataPath, "test.map");

		using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create)))
		{
			writer.Write(mapFileVersion);
			hexGrid.Save(writer);
		}
	}

	public void Action ()
	{
		string path = GetSelectedPath();
		if (path == null)
		{
			return;
		}
		if (saveMode)
		{
			Save(path);
		}
		else
		{
			Load(path);
		}
		Close();
	}


	public void Load(string path)
	{
		//MÉTODO RESPONSAVEL POR CARREGAR O MAPA A PARTIR DE UM ARQUIVO NA PASTA DO PROJETO.
		if (!File.Exists(path))
		{
			Debug.LogError("File does not exist " + path);
			return;
		}

		using (BinaryReader reader = new BinaryReader(File.OpenRead(path)))
		{
			int header = reader.ReadInt32();
			if (header <= mapFileVersion)
			{
				hexGrid.Load(reader, header);
				HexMapCamera.ValidatePosition();
			}
			else
			{
				Debug.LogWarning("Unknown map format " + header);
			}
		}
	}

	public void SelectItem (string name)
	{
		nameInput.text = name;
	}


	public void Delete()
	{
		string path = GetSelectedPath();
		if (path == null)
		{
			return;
		}

		if (File.Exists(path))
		{
			File.Delete(path);
		}
		nameInput.text = "";
		FillList();
	}
}



