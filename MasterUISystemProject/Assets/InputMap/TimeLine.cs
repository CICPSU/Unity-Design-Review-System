using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TimeLine : MonoBehaviour 
{
	public Vector2 scale = new Vector2 (.02f, .2f);
	public int capacity = 300;          

	LineRenderer lr = null;

	private List<float> values = new List<float>();

	public void AddPt( float value )
	{
		while( values.Count >= capacity )
			values.RemoveAt( capacity - 1 );
		values.Insert( 0, value );

		if ( lr != null )
		{
			lr.SetVertexCount( values.Count );

			for( int i=0; i<values.Count; i++ )
				lr.SetPosition(i, new Vector3( i * scale.x, values[i] * scale.y, 0f ) );
		}
	}



	// Use this for initialization
	void Start () 
	{
		gameObject.AddComponent<LineRenderer> ();
		lr = gameObject.GetComponent<LineRenderer> ();
		lr.SetWidth (.02f, .02f);
		lr.SetColors (Color.white, Color.red);
		lr.useWorldSpace = false;
	}
}
