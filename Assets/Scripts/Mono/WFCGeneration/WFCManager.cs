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

    // Method to start the generation process by checking for uncollapsed cells and selecting the next room until the generation is complete or fails.
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
