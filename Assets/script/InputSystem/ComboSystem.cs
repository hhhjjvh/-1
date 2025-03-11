using UnityEngine;
using System.Collections.Generic;
using System.Linq;



public class ComboSystem : MonoBehaviour
{
    // private static List<string> pressedKeys = new List<string>(); // ��ΪList�������¼�����ͬ�İ���


   private static List<List<string>> comboSkills = new List<List<string>>()
  {
      new List<string> { "Move_Left", "Move_Left" },
      new List<string> { "Move_Right", "Move_Right" },
  };

    public static void RegisterKey(string actionName)
    {
        CheckCombo();
    }


    private static void CheckCombo()
    {
        List<string> currentKeys = InputManager.Instance.GetPressedKeys();
        foreach (var combo in comboSkills)
        {
            if (IsComboMatched(currentKeys, combo))
            {
                InputManager.Instance.RestKeyPressEntries();
               // Debug.Log("���ܴ�����" + string.Join(" + ", combo));
                TriggerSkill(combo);
                return;
            }
        }
    }
    // �ж��Ƿ�����������������ǰ�����˳���������

    private static bool IsComboMatched(List<string> currentKeys, List<string> combo)
    {
        if (currentKeys.Count < combo.Count) return false;

        // ����Ƿ���������İ�������ƥ������
        for (int i = 0; i <= currentKeys.Count - combo.Count; i++)
        {
            bool isMatch = true;
            for (int j = 0; j < combo.Count; j++)
            {
                if (currentKeys[i + j] != combo[j])
                {
                    isMatch = false;
                    break;
                }
            }
            if (isMatch) return true;
        }
        return false;
    }

    private static void TriggerSkill(List<string> combo)
    {
       // Debug.Log("�������У�" + string.Join(" + ", combo));
        int index = comboSkills.FindIndex(c => c.SequenceEqual(combo));
        switch (index)
        {
           
            case 0:
            case 1: ChangeStateWithRun(); break;

            default:
                Debug.Log("δ֪����");
                break;
        }
      //  pressedKeys.Clear();
    }

  
    private static void ChangeStateWithRun()
    {
        if (PlayerManager.instance?.player == null) return;
        Player player = PlayerManager.instance.player;
        player.stateMachine.ChangeState(player.runState);
    }
    
   
}
