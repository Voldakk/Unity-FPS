using UnityEngine;

[System.Serializable]
public class Quality
{
    [HideInInspector]
    public int index;

    public string name;
    public Color color;
    public float modifier;
    [Range(0, 1)]
    public float rarity;
    
    public string calculatedChance;
}

public class Qualities : MonoBehaviour
{
    public static Qualities instance;

    public Quality[] qualities;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnValidate()
    {
        float chance = 1;
        int index = 0;
        foreach (var q in qualities)
        {
            q.index = index++;
            q.calculatedChance = (chance * q.rarity * 100f).ToString("0.00") + "%";
            chance *= 1.0f - q.rarity;
        }
    }

    public static Quality[] GetQualities()
    {
        return instance.qualities;
    }

    public static Quality GetRandomQuality()
    {
        foreach (var q in instance.qualities)
        {
            if (q.rarity >= Random.value)
                return q;
        }

        return instance.qualities[0];
    }

    public static Quality GetQuality(int index)
    {
        if (index >= 0 && index < instance.qualities.Length)
            return instance.qualities[index];

        return null;
    }
}
