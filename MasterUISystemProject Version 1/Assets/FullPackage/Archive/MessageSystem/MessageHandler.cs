using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public static class MessageHandler 
{
	public static Dictionary<string, GameObject> messageLists = new Dictionary<string, GameObject>();

	private static Dictionary<string, StreamWriter> streamWriters = new Dictionary<string, StreamWriter>();

	// So the PrintSystemMessage function will do the actual printing
	// there will be multiple signatures of the SystemMessage function, which calls PrintSystemMessage
	// This makes it easier to have multiple signatures so that you dont have to pass everything whenever you want to print a message.

	// this is the signature that lets the user give the message to be printed with default config
	public static void Message(string list, string message)
	{
		PrintMessage(list, message, null, null, null, null, null, null, null);
	}
	public static void Message(string list, string message, int? messageTime)
	{
		PrintMessage(list, message, messageTime, null, null, null, null, null, null);
	}

	// this signature lets the user configure the message height, color and margin
	public static void Message(string list, string message, int? messageTime,  Color messageColor, int? messageMargin)
	{
		PrintMessage(list, message, messageTime,  messageColor, messageMargin, null, null, null, null);
	}

	// this signature lets you configure the font color, size, style and anchor

	public static void Message(string list, string message, int? messageTime, Color fontColor, int? fontSize, FontStyle fontStyle, TextAnchor textAnchor)
	{
		MessageListManager mLM = messageLists[list].GetComponent<MessageListManager>();
		PrintMessage(list, message, messageTime, mLM.defaultBackgroundColor, mLM.defaultMargin, fontColor, fontSize, fontStyle, textAnchor);
	}

	// this signature lets you configure both the font and the message
	public static void Message(string list, string message, int? messageTime, Color messageColor, int? messageMargin, Color fontColor, int? fontSize, FontStyle fontStyle, TextAnchor textAnchor)
	{
		PrintMessage(list, message, messageTime, messageColor, messageMargin, fontColor, fontSize, fontStyle, textAnchor);
	}

	public static void RegisterList(string name, GameObject list)
	{
		messageLists.Add(name, list);
		StreamWriter sw = new StreamWriter(list.GetComponent<MessageListManager>().logFilePath, false);
		streamWriters.Add(name, sw);
		streamWriters[name].AutoFlush = true;
	}

	private static void PrintMessage(string list, string message, int? messageTime, Color? messageColor, int? margin, Color? fontColor, int? fontSize, FontStyle? style, TextAnchor? anchor)
	{
		try{
			if(!messageLists.ContainsKey(list))
				throw new Exception("Cannot Find List: " + list);

			MessageListManager mLM = messageLists[list].GetComponent<MessageListManager>();

			GameObject newMessage = MonoBehaviour.Instantiate(mLM.messagePrefab) as GameObject;

			newMessage.GetComponent<Image>().color = messageColor ?? mLM.defaultBackgroundColor;
			newMessage.GetComponent<MessageTimer>().messageSeconds = messageTime ?? mLM.defaultTime;

			Text newMessageText = newMessage.GetComponentInChildren<Text>();
			newMessageText.text = message;
			newMessageText.color = fontColor ?? mLM.defaultFontColor;
			newMessageText.fontSize = fontSize ?? mLM.defaultFontSize;
			newMessageText.fontStyle = style ?? mLM.defaultFontStyle;
			newMessageText.alignment = anchor ?? mLM.defaultTextAnchor;

			newMessage.transform.SetParent(mLM.transform, false);
			if(mLM.fillFromTopDown)
				newMessage.transform.SetAsFirstSibling();
			if(mLM.writeToLogFile)
			{
				StackTrace trace = new StackTrace(2, false);
				streamWriters[list].Write(Time.realtimeSinceStartup + ": "  + " Type and Method: " + trace.GetFrame(0).GetMethod() + System.Environment.NewLine + message + System.Environment.NewLine + System.Environment.NewLine);
			}
		}
		catch(Exception ex)
		{
			//UnityEngine.Debug.LogError(ex.Message);
			//UnityEngine.Debug.LogWarning("Could not find the specified message list, printing message to Debug.log instead");
			UnityEngine.Debug.Log(message);
		}
	}

	public static void FadeMessage(GameObject message)
	{
		// instead of destroying the message, we are just going to make it transparent
		//MonoBehaviour.Destroy(message);
		Image messageBackground = message.GetComponent<Image>();
		messageBackground.color = new Color(messageBackground.color.r,messageBackground.color.g,messageBackground.color.b, 0.25f);
		Text messageText = message.GetComponentInChildren<Text>();
		messageText.color = new Color(messageText.color.r,messageText.color.g,messageText.color.b, 0.25f);
	}

	public static void RemoveMessage(GameObject message)
	{
		MonoBehaviour.Destroy(message);
		if(message.transform.parent.GetComponent<RectTransform>().sizeDelta.y <= message.transform.parent.parent.GetComponent<RectTransform>().sizeDelta.y)
			message.transform.parent.GetComponent<MessageListManager>().messageScrollbarRef.GetComponent<Scrollbar>().value = 1;
	}
}
