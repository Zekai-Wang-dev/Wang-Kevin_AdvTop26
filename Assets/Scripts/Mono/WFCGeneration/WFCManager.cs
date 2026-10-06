using UnityEngine;

public class WFCManager : MonoBehaviour
{

    public WFCGenerator wfcGenerator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        wfcGenerator.GenerateCells();

        StartGeneration(); 

    }

    public void StartGeneration()
    {

        bool generationComplete = wfcGenerator.CheckForUncollapsedCells();

        while (!generationComplete)
        {

            wfcGenerator.SelectNextRoom();

            if (wfcGenerator.failed)
            {

                Debug.Log("Generation failed.");
                break; 

            }

            generationComplete = wfcGenerator.CheckForUncollapsedCells();

        }

    }

}
