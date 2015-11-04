using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.EventSystems;

public class MessageListManager : MonoBehaviour {

	/// <summary>
	/// This is the name of the list that is used to register it and to determine where messages should be displayed.
	/// </summary>
	public string listName;

	/// \name MessageListDefaults
	//@{ 
	/// <summary>
	/// This is the default time that a message will be on the screen. Measured in seconds.
	/// </summary>
	public int defaultTime = 5;

	/// <summary>
	/// This is the default height of a message when it is displayed in this message list.  Measured in pixels.
	/// </summary>
	public int defaultMessageHeight = 50;

	/// <summary>
	/// This is the defualt font size of a message when it is displayed in this message list.
	/// </summary>
	public int defaultFontSize = 14;

	/// <summary>
	/// This is the default margin between the message's text and the edge of the message box for a message displayed in this message list.
	/// </summary>
	public int defaultMargin = 0;

	/// <summary>
	/// This is the default color of the font in a message displayed in this message list.
	/// </summary>
	public Color defaultFontColor = Color.white;

	/// <summary>
	/// This is the default color of the background of a message that is displayed in this message list.
	/// </summary>
	public Color defaultBackgroundColor = Color.black;

	/// <summary>
	/// This is the default FontStyle that messages displayed in this message list will use.
	/// </summary>
	public FontStyle defaultFontStyle = FontStyle.Normal;

	/// <summary>
	/// This is the default TextAnchor that messged displayed in this message list will use. The TextAnchor describes where the text appears inside a Text object.
	/// </summary>
	public TextAnchor defaultTextAnchor = TextAnchor.UpperLeft;
	//@}

	/// <summary>
	/// This is the prefab that will be used to instantiate a new message.
	/// </summary>
	public GameObject messagePrefab;

	/// <summary>
	/// When true, the message list will place new messages at the top of the message list.
	/// </summary>
	public bool fillFromTopDown = false;

	/// <summary>
	/// When true, messages sent to this message list will also be written to a log file.
	/// </summary>
	public bool writeToLogFile = false;

	/// <summary>
	/// This string is the path to the log file that this message list can write to.
	/// </summary>
	public string logFilePath = null;

	/// <summary>
	/// This is a reference to the Mask object for this message list.  If the list is longer than the mask, we display the scroll bar so that the user can access all of the messages.
	/// </summary>
	public GameObject messageMaskRef = null;

	/// <summary>
	/// This is a reference to the actual UI element that is the message list.  Messages instantiated by this list become children of this gameobject.
	/// </summary>
	public GameObject messageListRef = null;

	/// <summary>
	/// This is a reference to the scrollbar associated with this message list.  We show and hide the scroll bar based on the size of the list and mask objects.
	/// </summary>
	public GameObject messageScrollbarRef = null;

	/// <summary>
	/// This bool is true while the scrollbar is selected by the user.  We use this so that messages don't disappear while the user is scrolling and reading them.
	/// </summary>
	private bool scrollBarSelected = false;

	// Use this for initialization
	void Awake () {
		if(logFilePath == null || logFilePath == "")
			logFilePath =  Application.persistentDataPath + "/" + listName + "_Log.txt";
		MessageHandler.RegisterList(listName, this.gameObject);
	}

	/// <summary>
	/// This public function allows us to toggle the scrollBarSelected variable so that we know when the user is actively selecting the scrollbar.
	/// </summary>
	public void ToggleScrollbarSelected()
	{
		scrollBarSelected = !scrollBarSelected;
	}

	void Update()
	{
		messageScrollbarRef.SetActive(messageMaskRef.GetComponent<RectTransform>().sizeDelta.y < messageListRef.GetComponent<RectTransform>().sizeDelta.y || scrollBarSelected);
	}
}
