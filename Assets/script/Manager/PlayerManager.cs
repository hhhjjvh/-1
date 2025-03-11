using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour,ISaveManager
{
    public static PlayerManager instance;
    public Player player;

    public int currency;
    public int Coin;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
        }
        instance = this;
    }
    public void addCoin(int coin)
    {
        Coin += coin;
      
    }
    public void removeCoin(int coin)
    {
        Coin -= coin;
     
    }
    public bool HaveEnoughMoney(int amount)
    {
       if(amount> currency)
        {
          
            return false;
        }
       currency -= amount;
      
        return true;
    }
    public bool HaveEnoughCoin(int amount)
    {
        if (amount > Coin)
        {
        
            return false;
        }
        Coin -= amount;
      
        return true;
    }
    public void AddMoney(int amount)
    {
        currency += amount;

    }

    public int GetCurrency()
    {
        return currency;
    }
    public int GetCoin()
    {
        return Coin;
    }

    public void LoadData(GameData data)
    {
       // Debug.Log("load");
        currency = data.currency;
        Coin = data.coin;
    }

    public void SaveData(ref GameData data)
    {
       // Debug.Log("save");
       data.currency = currency;
       data.coin = Coin;
    }
}
