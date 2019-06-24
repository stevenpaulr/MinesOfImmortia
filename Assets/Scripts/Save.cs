using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Save 
{

	public Dictionary<string, int> stats = new Dictionary<string, int>();
	public List<string> attributes = new List<string>();
	public int HP;
	public int maxHP;
	public int doomClock;
	public bool plus1Forward = false;
	public bool minus1Forward = false;

    public string currentEventKey;

	public List<string> dialogHistory = new List<string>();

    public int dialogRandNum;

}
