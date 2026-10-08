using UnityEngine;

public class WFCManager : MonoBehaviour
{

    public WFCGenerator wfcGenerator;

    public int retries = 0; 

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

            Debug.Log("Selecting next room...");

            wfcGenerator.SelectNextRoom();

            if (wfcGenerator.failed)
            {

                Debug.Log("Generation failed.");
                wfcGenerator.GenerateCells();
                wfcGenerator.failed = false;
                retries++;
                Debug.Log("Retrying generation. Attempt: " + retries);
                StartGeneration();
                
                break; 

            }

            generationComplete = wfcGenerator.CheckForUncollapsedCells();

        }

        if (wfcGenerator.failed || !generationComplete)
            return;

        if (!wfcGenerator.CheckAllRoomsConnectivity())
        {
            Debug.Log("Generation failed due to connectivity issues.");
            wfcGenerator.failed = true;
            wfcGenerator.generationComplete = false;

            return;
        }

    }

}
