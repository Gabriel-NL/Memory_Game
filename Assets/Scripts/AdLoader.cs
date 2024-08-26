using UnityEngine;
using UnityEngine.Advertisements;

public class AdLoader : MonoBehaviour
{
    
    string gameId = "5679776";
    bool testMode = true;

    void Start () {
        Advertisement.Initialize (gameId, testMode);
    }

}
