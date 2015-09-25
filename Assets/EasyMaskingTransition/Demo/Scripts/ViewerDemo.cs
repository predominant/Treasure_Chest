using UnityEngine;
using System.Collections;

[RequireComponent(typeof(EMTransition))]
public class ViewerDemo : MonoBehaviour
{
	[SerializeField] Texture2D[] gradations;
	[SerializeField] Texture2D[] textures;
	[SerializeField] int current = 0;
	EMTransition emTransition;

	void Start()
	{
		emTransition = GetComponent<EMTransition>();
		emTransition.SetGradationTexture(gradations[current]);
	}

	void Update()
	{
		if(!gradations[current]) return;
		
		// switch gradation texture to the next one.
		if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			current = (current-- > 0) ? current : gradations.Length - 1;
			emTransition.SetGradationTexture(gradations[current]);
		}
		
		// switch gradation texture to the prev one.
		if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			current = (++current < gradations.Length) ? current : 0;
			emTransition.SetGradationTexture(gradations[current]);
		}
	}
	
	void OnGUI()
	{
		if(!gradations[current]) return;
		
		GUI.Label (new Rect (20, 10, 100, 20), "GRADATION:");

		// switch gradation texture to the next one.
		if (GUI.Button(new Rect(110, 10, 30, 20), "<"))
		{
			current = (current-- > 0) ? current : gradations.Length - 1;
			emTransition.SetGradationTexture(gradations[current]);
		}
		
		// switch gradation texture to the prev one.
		if (GUI.Button(new Rect(150, 10, 30, 20), ">"))
		{
			current = (++current < gradations.Length) ? current : 0;
			emTransition.SetGradationTexture(gradations[current]);
		}

		// indicate texture name
		GUI.Label (new Rect (190, 10, 200, 20), gradations[current].name + " / 040");
		
		GUI.Label (new Rect (20, 40, 100, 20), "COLOR:");

		// switch color
		if (GUI.Button(new Rect(110, 40, 80, 20), "black")) emTransition.SetColor(Color.black);
		if (GUI.Button(new Rect(200, 40, 80, 20), "white")) emTransition.SetColor(Color.white);
		if (GUI.Button(new Rect(290, 40, 80, 20), "red")) emTransition.SetColor(Color.red);
		if (GUI.Button(new Rect(380, 40, 80, 20), "green")) emTransition.SetColor(Color.green);
		if (GUI.Button(new Rect(470, 40, 80, 20), "blue")) emTransition.SetColor(Color.blue);
		if (GUI.Button(new Rect(560, 40, 80, 20), "random"))
		{
			Color random = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
			emTransition.SetColor(random);
		}

		GUI.Label (new Rect (20, 70, 100, 20), "TEXTURE:");

		// switch texture
		if (GUI.Button(new Rect(110, 70, 80, 20), "none")) 
		{
			emTransition.SetTexture(textures[0]);
		}

		if (GUI.Button(new Rect(200, 70, 80, 20), "tile"))
		{
			emTransition.SetTexture(textures[1]);
			emTransition.SetColor(Color.white);
		}

		if (GUI.Button(new Rect(290, 70, 80, 20), "wood"))
		{
			emTransition.SetTexture(textures[2]);
			emTransition.SetColor(Color.white);
		}
	}
}
