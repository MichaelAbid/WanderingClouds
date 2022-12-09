using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace WanderingCloud
{
    public class StartingThings : MonoBehaviour
    {

        public string MainMenu = "MainMenu";

        // Start is called before the first frame update
        void Start()
        {


            SceneManager.LoadScene(MainMenu);

        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
