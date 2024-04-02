using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DialogueSystem.Editor.Elements;
using DialogueSystem.Editor.Utilities;
using DialogueSystem.Editor.Window;
using UnityEditor;
using UnityEngine;

namespace DialogueSystem.Data
{
	public sealed class NodeSaveData : SaveData
	{
		public DialogueNode Node { get; set; }

		//TODO: try to reduce this repetition
		public override string Name
		{
			get => name;
			set
			{
				previousName = name;
				previousPath = Path.Combine(DialogueGraphView.C.GraphPath, Group == null ? "Ungrouped" : $"Groups/{Group.Name}", $"{previousName}.asset").ReplaceSlash();

				//Setting name directly does not work. RenameAsset() sets name for us.
				string newName = String.IsNullOrWhiteSpace(value) ? Id : value.Trim();
				folderPath = Path.Combine(DialogueGraphView.C.GraphPath, Group == null ? "Ungrouped" : $"Groups/{Group.Name}").ReplaceSlash();
				path = Path.Combine(folderPath, $"{newName}.asset").ReplaceSlash();

				if (!File.Exists(path) && !File.Exists(previousPath))
				{
					AssetDatabase.CreateAsset(this, path);
				}
				else if (previousName != newName)
				{
					AssetDatabase.RenameAsset(previousPath, newName);
					OnReinitialize();
				}
				else if (previousPath != path)
				{
					AssetDatabase.MoveAsset(previousPath, path);
				}
			}
		}

		public string Text = "Text";

		public NodeSaveData Next;
		public List<ChoiceSaveData> Choices = new();

		[SerializeField]
		private GroupSaveData group;
		public GroupSaveData Group
		{
			get => group;
			set
			{
				group = value;
				previousName = name;
				previousPath = path;
				folderPath = Path.Combine(DialogueGraphView.C.GraphPath, Group == null ? "Ungrouped" : $"Groups/{Group.Name}").ReplaceSlash();
				path = Path.Combine(folderPath, $"{name}.asset").ReplaceSlash();

				AssetDatabase.MoveAsset(previousPath, path);
			}
		}

		public static NodeSaveData Create(Vector2 position) => SaveData.Create<NodeSaveData>(position);

		public override void Delete() => AssetDatabase.DeleteAsset(path);

		///When the name of a SO changes, Unity reinitializes the SO and copies the values of its members. Because of this, references to reference types (in this case the choices) become stale. I hate this and I hate Unity.
		private void OnReinitialize()
		{
			if (Node?.ChoicesDisplay == null || Choices.Count == 0)
				return;

			var children = Node.ChoicesDisplay.Children.ToArray();
			for (int i = 0; i < children.Length; i++)
				children[i].SaveData = Choices[i];
		}
	}
}