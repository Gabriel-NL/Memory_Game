using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardList : MonoBehaviour
{
    // Start is called before the first frame update
    public List<Sprite> cards=new List<Sprite>();
    public Sprite card_back;

    public Sprite Get_card(int id){
        try
        {
            return cards[id];
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Deu erro: " + e.Message);
            throw;
        }
    }
}
