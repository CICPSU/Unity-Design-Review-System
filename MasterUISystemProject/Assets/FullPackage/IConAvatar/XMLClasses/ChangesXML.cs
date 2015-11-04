using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;
using System;

/* ValueToChange:
 * This class holds the information that points to a member in a script to be modified.
 * 
 * Variables:
 * path: string. path to the gameobject that the script is attached to. relative to the DIREController object ex: "DIREController/Head"
 * 
 * script: string. name of the script that contains the member to be modified.
 * 
 * field: string. name of the field to modify
 * 
 * newValue: object (generic so that it can hold values for different field types).  new value of the field.
 */

public class ValueToChange{
	[XmlElement("Path")]
	public string path{get; set;}
	
	[XmlElement("ScriptToChange")]
	public string script{get; set;}
	
	[XmlElement("FieldToChange")]
	public string field{get; set;}
	
	[XmlElement("NewValue")]
	public object newValue{get; set;}

	public ValueToChange(){

	}// default ValueToChange contructor

	public ValueToChange(string p, string sc, string f, object newV){
		path = p;
		script = sc;
		field = f;
		newValue = newV;
	}// ValueToChange contructor
}// Hierarchy

/* VariableLoader:
 * This class holds a list of ValueToChange.  These are all of the modifications that need to be made to scripts on objects in the DIREController's heirarchy.
 */
[XmlRoot("VariableLoader")]

public class VariableLoader{
	[XmlArray("ValuesToChange")]
	public List<ValueToChange> values = new List<ValueToChange>();

	public int CheckForField(FieldInfo fIn){
		int i = 0;
		foreach(ValueToChange val in values){
			if(val.field == fIn.Name){
				return i;
			}// name if
			i++;
		}// foreach
		return -1;
	}// CheckForField

	public VariableLoader(){
		values = new List<ValueToChange>();

	}// default VariableLoader contructor

	public VariableLoader(List<ValueToChange> vTCList){
		values = vTCList;
	}// VariableLoader contructor
	
}// variableLoader