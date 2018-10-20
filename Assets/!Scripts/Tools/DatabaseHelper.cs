using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;

public class DatabaseHelper<DBType> where DBType : ScriptableObject
{
    //Helper class for managing and querying collections of ScriptableObjects (i.e. ModTemplates)

    public static readonly string RESOURCES_DIRECTORY_PATH = Application.dataPath + "/Resources/";  //it's like a const, but not

    protected Dictionary<string, DBType> _database = new Dictionary<string, DBType>();      //actual database of assets
    protected readonly string _path;           //where below the resources folder are these assets located?
    protected readonly string _assetTypeName;  //for debug log, what kind of assets are these?

    //constructor
    public DatabaseHelper(string path, string assetTypeName)
    {
        _path = path;
        _assetTypeName = assetTypeName;
    }

    //methods
    public bool Load()
    {
        //Load all assets of the specified type from the specified directory into a managed dictionary
        //return true if load was successful
        //call this during game initialization

        //resource path
        var targetPath = RESOURCES_DIRECTORY_PATH + _path;

        //check to see if directory even exists
        if (!Directory.Exists(targetPath))
        {
            Debug.LogError("Unable to resolve path for " + _assetTypeName + " database: " + targetPath);
            return false;
        }
        else
        {
            Debug.Log("Loading " + _assetTypeName + " database: " + targetPath);
        }

        //empty the current database in case we call this multiple times
        _database.Clear();

        //load all assets of specified type and add them to the dictionary (key is the ScriptableObject's Unity name in CAPS, like "MY MOD TEMPLATE 03")
        new List<DBType>(Resources.LoadAll<DBType>(_path))
            .ForEach(loaded => _database.Add(loaded.name.ToUpper(), loaded));

        //declare database count
        Debug.Log(string.Format("Just loaded all {0} entries in {1} database.", _database.Count, _assetTypeName));

        //if the database is empty, then why do we even have it? something must have gone wrong.
        if (_database.Count == 0)
            return false;

        //everything went right, return true
        return true;
    }
    public bool TryFind(string identifierString, out DBType foundAsset)
    {
        if (_database.TryGetValue(identifierString.ToUpper(), out foundAsset))
            return true;
        return false;
    }
    public bool TryFindMany(System.Predicate<DBType> predicate, out List<DBType> foundAssets)
    {
        //legible version
        foundAssets = new List<DBType>();
        foreach (KeyValuePair<string, DBType> kvp in _database)
        {
            if (predicate(kvp.Value))
            {
                foundAssets.Add(kvp.Value);
            }
        }
        
        //illegible version
        //foundAssets = new List<DBType>(_database.ToList().Where(kvp => predicate(kvp.Value)).Select(kvp => kvp.Value));

        if (foundAssets.Count > 0)
            return true;
        return false;
    }
    
    public bool TryGetRandom(out DBType randomValue)
    {
        //call method below, but the predicate is "true" meaning all things will match it (proud of how efficiently lazy this is)
        return TryGetRandom((dbVal) => true, out randomValue);
    }

    public bool TryGetRandom(System.Predicate<DBType> predicate, out DBType randomValue)  //where the asset matches the predicate
    {
        //get a random value from subset of all values matching the predicate
        List<DBType> found;
        if (TryGetRandomFromSubset(_database.ToList().Where(kvp => predicate(kvp.Value)).ToList(), out found))
        {
            randomValue = found[0]; //first element in a list guaranteed to have exactly 1 item.
            return true;
        }
        else
        {
            randomValue = null;
            return false;
        }
    }
    public bool TryGetRandom(System.Predicate<DBType> predicate, out List<DBType> randomValues, int count = 1)
    {
        //prepare a subset of values which match the predicate, then pass to the method below
        return TryGetRandomFromSubset(_database.ToList().Where(kvp => predicate(kvp.Value)).ToList(), out randomValues, count);
    }

    protected bool TryGetRandomFromSubset(List<KeyValuePair<string, DBType>> subset, out List<DBType> randomValues, int count = 1)
    {
        randomValues = new List<DBType>();

        if (subset.Count() > 0)
        {
            //check if not enough options in subset
            if (subset.Count < count)
            {
                //if so, give as many as we have
                count = subset.Count;
            }

            //for as many times as we need values
            for (int i = 0; i < count; i++)
            {
                //pick a random value from the subset
                int roll = Random.Range(0, subset.Count()); //max exclusive

                //add it to the list of selected values
                randomValues.Add(subset.ElementAt(roll).Value);

                //remove it from the list of choices
                subset.RemoveAt(roll);
            }

            //all values have already been assigned, return true
            return true;
        }

        //return false because there were no values to select from
        return false;
    }
}
