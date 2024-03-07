using System;
using DialogueSystem.Editor.Elements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueSystem.Editor.Extensions
{
	///Lots of extensions to VisualElements and their inheritors.
	public static class ElementExtensions
	{
		#region Buttons
		public static Button CreateButton(string text, Action onClick) => new(onClick) {text = text};

		public static void InsertButton(this VisualElement element, int index, string text, Action onClick) => element.Insert(index, CreateButton(text, onClick));

		public static void AddButton(this VisualElement element, string text, Action onClick) => element.Add(CreateButton(text, onClick));

		public static Button CreateIconButton(string icon, Action onClick)
		{
			Button button = new(onClick);
			button.AddStyleSheet("Inputs/IconButton");
			button.AddToClassList("iconButton");
			button.Add(new Image {sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/DialogueSystem/Editor/Window/Icons/{icon}.png")});
			return button;
		}

		public static void InsertIconButton(this VisualElement element, int index, string icon, Action onClick) => element.Insert(index, CreateIconButton(icon, onClick));

		public static void AddIconButton(this VisualElement element, string icon, Action onClick) => element.Add(CreateIconButton(icon, onClick));
		#endregion

		#region Port
		public static Port CreatePort(Direction direction, Port.Capacity capacity)
		{
			var port = Port.Create<Edge>(Orientation.Horizontal, direction, capacity, typeof(bool));
			port.portName = "";
			port.AddStyleSheet("Inputs/Port");
			return port;
		}

		public static void InsertPort(this VisualElement element, int index, Direction direction, Port.Capacity capacity) => element.Insert(index, CreatePort(direction, capacity));

		public static void AddPort(this VisualElement element, Direction direction, Port.Capacity capacity) => element.Add(CreatePort(direction, capacity));
		#endregion

		#region TextField
		public static TextField CreateTextField(EventCallback<ChangeEvent<string>> onChange, string value = "", bool multiline = false, string label = "")
		{
			TextField textField = new() {value = value, label = label, multiline = multiline};
			textField.AddStyleSheet("Inputs/TextInput");
			textField.RegisterValueChangedCallback(onChange);
			return textField;
		}

		public static void InsertTextField(this VisualElement element, int index, EventCallback<ChangeEvent<string>> onChange, string value = "", bool multiline = false, string label = "") => element.Insert(index, CreateTextField(onChange, value, multiline, label));

		public static void AddTextField(this VisualElement element, EventCallback<ChangeEvent<string>> onChange, string value = "", bool multiline = false, string label = "") => element.Add(CreateTextField(onChange, value, multiline, label));
		#endregion
		
        public static void AddStyleSheet(this VisualElement element, string styleSheetName) => element.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>($"Assets/DialogueSystem/Editor/Window/USS/{styleSheetName}.uss"));

		public static DialogueNode GetStartNode(this Edge edge) => (DialogueNode) edge.output.node;
		public static DialogueNode GetEndNode(this Edge edge) => (DialogueNode) edge.input.node;

		public static void Show(this VisualElement element) => element.style.display = DisplayStyle.Flex;
		public static void Hide(this VisualElement element) => element.style.display = DisplayStyle.None;
	}
}