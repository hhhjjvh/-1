using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// <para>ScriptableObject��Ϊ���������࣬�̳���Ľű����ɲ��������κζ����ϣ���ʵ�����ݹ�������洢ͬһ���ݵĽű���δ����������ڴ�</para>
/// <para>�̳�ScriptableObject��Ľű��д洢�������ڱ༭��ģʽ�־ã�����󲻳־ã�һ��洢��Ϸ�����������в���ı�ı���</para>
/// </summary>
[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObject/ʰȡ��", order = 0)]
public class ItemData : ScriptableObject
{
    public string itemName; // ��Ʒ��
    public string itemDes; // ��Ʒ����
    public Sprite icon; // ��ƷͼƬ
    public ItemType itemType;
    public string itemID;
    public int buyPrice; // ����۸�
    public int sellPrice; // �����۸�
    public ItemEffect[] itemEffects;
    // public bool canUse; // �Ƿ�ɱ�ʹ��
    private void OnValidate()
    {


#if UNITY_EDITOR

        string path = AssetDatabase.GetAssetPath(this);
        itemID = AssetDatabase.AssetPathToGUID(path);
#endif
    }
    public void ItemEffect(Transform enemyPosition)
    {
        if (itemEffects == null) { return; }
        foreach (ItemEffect itemEffect in itemEffects)
        {
            itemEffect.ExecuteEffect(enemyPosition);
        }
    }
}
public enum ItemType
{
    Material,
    Equipment,
    food
}

public class ItemsConst // ��Ʒ��
{
    public const string Arrow = "��ʸ";
    public const string MushroomChunks = "Ģ�����";
    public const string LifePotion = "����ҩ��";
}