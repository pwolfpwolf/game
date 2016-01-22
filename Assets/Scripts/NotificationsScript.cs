using UnityEngine;
using System.Collections.Generic;

public class NotificationsScript : MonoBehaviour {

	private static List<Notification> noteList;

	private Rect windowRect;
	private Rect textRect;

	private bool noteVisible = false;
	private Notification currentNote;

	private static int MESSAGE_WIDTH = 300;
	private static int MESSAGE_HEIGTH = 200;

	void Start() {

		refresh();
	}
	
	void OnGUI () {
		windowRect  = new Rect (Screen.width - 200, 20, 180 , 80 + (noteList.Count * 20));
		windowRect = GUI.Window (0, windowRect, WindowFunction, "Messages");

		if (noteVisible) {
			displayNote();
		}
	}
	
	private void WindowFunction (int windowID ) {
		if(GUI.Button(new Rect(10,25,80,20), "Delete All")) {

			refresh();
		}

		listNotes();
	}



	private  void listNotes() {

		int startY = 55;

		foreach (Notification note in noteList) {

			if(GUI.Button(new Rect(10,startY,80,20), note.Subject)) {
				showNote(note);
			}
			string read; if (note.Read) read = "Read"; else read = "New";
			GUI.Label( new Rect(90,startY,50,20), read);
			if(GUI.Button(new Rect(120,startY,20,20), "X")) {
				Requester.deleteNote(note.NoteNumber);
				noteVisible = false;
				noteList.Remove(note);
			}

			startY += 20;
		}
	}

	private void showNote(Notification note) {

		noteVisible = true;
		currentNote = note;
		if (currentNote.Read == false) {
			currentNote.Read = true;
			Requester.readNote(currentNote.NoteNumber);
		}
	}

	public static void refresh() {

		noteList = Requester.getNotifications();
	}

	private void displayNote() {

		int leftX = (Screen.width / 2) - (MESSAGE_WIDTH / 2);
		int topY = (Screen.height / 2) - (MESSAGE_HEIGTH / 2);

		textRect  = new Rect (leftX, topY, MESSAGE_WIDTH , MESSAGE_HEIGTH);
		textRect = GUI.Window (1, textRect, TextWindowFunction, currentNote.Subject);
	}

	private void TextWindowFunction (int windowID ) {

		if(GUI.Button(new Rect(2,2,20,20), "X")) {
			noteVisible = false;
		}

		string textStr = convertFromHTML(currentNote.Text);

		GUI.TextArea (new Rect (2 , 20, MESSAGE_WIDTH - 4, MESSAGE_HEIGTH - 20), textStr);
	}

	private string convertFromHTML(string htmlStr) {

		string result = htmlStr.Replace("<br>", "\n");
		result = result.Replace("<BR>", "\n");

		return result;
	}
}
