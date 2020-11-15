using System.Collections.Generic;

[System.Serializable]
public class Data
{
    // Is it the first time the player is playing?
    private bool firstTime;

    // List containing completed songs' id
    private List<int> songIDList;
    // List containing completed songs' records
    private List<int[]> songRecords;

    public Data(bool firstTime, Dictionary<int, int[]> records)
    {
        this.firstTime = firstTime;

        foreach(int n in records.Keys)
        {
            songIDList.Add(n);
        }

        foreach (int[] array in records.Values)
        {
            songRecords.Add(array);
        }
    }
}
