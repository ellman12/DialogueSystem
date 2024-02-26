using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueSystem.Editor.Utilities
{
	///Allows easy creation of UI elements.
	public static class ElementUtility
	{
		public static Button CreateButton(string text, Action onClick) => new(onClick) { text = text };

		public static Button CreateIconButton(string icon, Action onClick)
		{
			Button button = new(onClick);
			button.AddStyleSheet("IconButton");
			button.AddToClassList("IconButton");
			button.Add(new Image {sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/DialogueSystem/Editor/Window/Icons/{icon}.png")});
			return button;
		}
		
		public static Foldout CreateFoldout(string title, bool collapsed = false) => new() { text = title, value = !collapsed };
		
		public static Port CreatePort(this Node node, Direction direction, Port.Capacity capacity)
		{
			var port = node.InstantiatePort(Orientation.Horizontal, direction, capacity, typeof(bool));
			port.portName = "";
			port.AddStyleSheet("Port");
			return port;
		}

		public static TextField CreateTextField(string value, string label, EventCallback<ChangeEvent<string>> onChange)
		{
			TextField textField = new() { value = value, label = label };
			textField.AddStyleSheet("TextInput");
			textField.RegisterValueChangedCallback(onChange);
			return textField;
		}

		public static TextField CreateTextArea(string value, string label, EventCallback<ChangeEvent<string>> onChange)
		{
			var textArea = CreateTextField(value, label, onChange);
			textArea.multiline = true;
			return textArea;
		}
	}
}