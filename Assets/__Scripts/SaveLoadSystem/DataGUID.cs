using UnityEngine;

[ExecuteAlways]
public class DataGUID : MonoBehaviour
{
    public string guid;

    private void Awake()
    {
        if (guid == string.Empty)
        {
            guid = System.Guid.NewGuid().ToString();
        }
    }
}