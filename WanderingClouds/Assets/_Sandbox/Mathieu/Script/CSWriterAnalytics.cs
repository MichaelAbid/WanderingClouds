using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CSWriterAnalytics : MonoBehaviour
{
    string filename = "";
    public int testNumber;

    [System.Serializable]
    public class PlayableCharacter
    {
        public int isUrle;
        public int isGiro;
        public int poofCollected;
        public int poofUsed;
        public int lianeTaked;
        public int cavityTaked;
    }

    [System.Serializable]
    public class PlayerList
    {
        public PlayableCharacter[] player;
    }

    public PlayerList playerList = new PlayerList();


    void Start()
    {
        filename = Application.dataPath + "/ExcelTest" + testNumber + ".csv";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            WriteCSV();
            Debug.Log("WriteCSV");
        }
    }

    public void WriteCSV()
    {
        if (playerList.player.Length > 0)
        {
            TextWriter tw = new StreamWriter(filename, false);
            tw.WriteLine("isUrle; isGiro; poofCollected; poofUsed; lianeTaked; cavityTaked");
            tw.Close();

            tw = new StreamWriter(filename, true);

            for (int i = 0; i < playerList.player.Length; i++)
            {
                tw.WriteLine(playerList.player[i].isUrle + ";" + playerList.player[i].isGiro);
            }
            tw.Close();
        }
    }
}
