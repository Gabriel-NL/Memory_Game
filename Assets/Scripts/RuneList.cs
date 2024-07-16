using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneList : MonoBehaviour
{
    // Start is called before the first frame update
    public List<Sprite> runes=new List<Sprite>();
    public Sprite rune_back;

    public Sprite Get_rune(int id){
        try
        {
            return runes[id];
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Deu erro: " + e.Message);
            throw;
        }
    }

    public int ElementCount(){
        return runes.Count;
    }
}
