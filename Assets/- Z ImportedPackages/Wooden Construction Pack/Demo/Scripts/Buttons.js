private var LevelsName : String[] = [ 
    "Cavern", 
    "Area",
    "Rocky Road" ] ;
private var label_play = ">";
var anim : Animator; 
private var buttonPressed : boolean = true;
function Start () {
	
	if (anim != null)
		anim.speed = 0;
	
}

function OnGUI() {
	
	for (var i = 0; i <= 2 ; i++)
	{
		GUI.color = Color.cyan; 
		if (i == Application.loadedLevel)
		{
 			GUI.enabled=false;
 			GUI.color = Color.white;
			GUI.Button(Rect((Screen.width / 2-600)+i*200,10,150,50), LevelsName[i]);
			GUI.enabled=true;
		} 
		else
		{
			if (GUI.Button(Rect((Screen.width / 2-600)+i*200,10,150,50), LevelsName[i]))
				Application.LoadLevel(i);
			if (anim != null)
			{
				if (GUI.Button(Rect((Screen.width / 2),(Screen.height / 2+300),50,50), label_play ))
				{
					if (buttonPressed)
						{ 
							label_play = "||";		
							anim.speed = 1;
							buttonPressed = false;
						}
						else
						{
							label_play = ">";		
							anim.speed = 0;
							buttonPressed = true;
						}
				}
				if (GUI.Button(Rect((Screen.width / 2-50),(Screen.height / 2+300),50,50), "<<"))
						anim.speed = -3;
				if (GUI.Button(Rect((Screen.width / 2+50),(Screen.height / 2+300),50,50), ">>"))
						anim.speed = 3;
			}
		}
		
	}
					
}