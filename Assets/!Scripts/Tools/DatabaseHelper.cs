﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable] public class UnityEventDatabaseHelper : UnityEvent<DatabaseHelper> { }
public abstract class DatabaseHelper
{
    protected readonly string _path;           //where below the resources folder are these assets located?
    protected readonly string _assetTypeName;  //for debug log, what kind of assets are these?

    //events
    public UnityEvent onStartLoad = new UnityEvent();
    public UnityEventInt onSuccessfulLoad = new UnityEventInt();
    public UnityEventString onFailedLoad = new UnityEventString();
    public UnityEventString onFailedToLocateAsset = new UnityEventString();

    //constructor
    public DatabaseHelper(string path, string assetTypeName, List<DatabaseHelper> containerList = null)
    {
        _path = path;
        _assetTypeName = assetTypeName;

        if (containerList != null)
            containerList.Add(this);
    }
    //methods
    public bool Load(bool logStart = true, bool logSuccess = true)
    {
        //Load all assets of the specified type from the specified directory into a managed dictionary
        //return true if load was successful
        //call this during game initialization

        if (onStartLoad != null)
            onStartLoad.Invoke();

        //resource path
        var targetPath = Application.dataPath + "/Resources/" + _path;

        //check to see if directory even exists
        if (!Directory.Exists(targetPath))
        {
            if (onFailedLoad != null)
                onFailedLoad.Invoke(targetPath);

            Debug.LogError("Unable to resolve path for " + _assetTypeName + " database: " + targetPath);
            return false;
        }
        else
        {
            if (logStart)
                Debug.Log("Loading " + _assetTypeName + " database: " + targetPath);
        }

        int count = Load_Internal();

        if (onSuccessfulLoad != null)
            onSuccessfulLoad.Invoke(count);

        //declare database count
        if (logSuccess)
        {
            //if the database is empty, then why do we even have it? something must have gone wrong.
            if (count == 0)
                Debug.Log(string.Format("{0} database was successfully loaded, but is empty.", _assetTypeName));
            else
                Debug.Log(string.Format("Just loaded all {0} asset(s) in {1} database.", count, _assetTypeName));
        }

        //everything went right, return true
        return true;
    }
    protected abstract int Load_Internal();  //return database.Count
}
public class DatabaseHelper<DBType> : DatabaseHelper where DBType : ScriptableObject
{
    //Helper class for managing and querying collections of ScriptableObjects (i.e. ModTemplates)

    protected Dictionary<string, DBType> _database = new Dictionary<string, DBType>();      //actual database of assets

    public DatabaseHelper(string path, string assetTypeName, List<DatabaseHelper> containerList = null) :base(path, assetTypeName, containerList) { }

    protected override int Load_Internal()   //return database.Count
    {
        //empty the current database in case we call this multiple times
        _database.Clear();

        //load all assets of specified type and add them to the dictionary (key is the ScriptableObject's Unity name in CAPS, like "MY MOD TEMPLATE 03")
        var foundAssets = new List<DBType>(Resources.LoadAll<DBType>(_path));
        int count = foundAssets.Count;
        if (count == 0)
            return count;

        foundAssets.ForEach(loaded => _database.Add(loaded.name.ToUpper(), loaded));
        return count;
    }
    public bool TryFind(string identifierString, out DBType foundAsset)
    {
        if (_database.TryGetValue(identifierString.ToUpper(), out foundAsset))
            return true;

        Debug.LogError("Database Error: Unable to locate asset with internal name: " + identifierString);

        if (onFailedToLocateAsset != null)
            onFailedToLocateAsset.Invoke(identifierString);

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

    public bool TryGetRandom(System.Predicate<DBType> predicate, out DBType randomAsset)  //where the asset matches the predicate
    {
        //get a random value from subset of all values matching the predicate
        randomAsset = null;

        List<DBType> foundAssets;
        if (TryGetRandom(predicate, out foundAssets))
        {
            randomAsset = foundAssets[0]; //first element in a list guaranteed to have exactly 1 item.
            return true;
        }
        return false;
    }
    public bool TryGetRandom(System.Predicate<DBType> predicate, out List<DBType> randomAssets, int count = 1)
    {
        //prepare a subset of values which match the predicate, then pass to the method below
        return TryGetRandomFromSubset(_database.Values.ToList().Where(asset => predicate(asset)).ToList(), out randomAssets, count);
    }

    protected bool TryGetRandomFromSubset(List<DBType> subset, out List<DBType> randomValues, int count = 1)
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
                randomValues.Add(subset.ElementAt(roll));

                //remove it from the list of choices
                subset.RemoveAt(roll);
            }

            //all values have already been assigned, return true
            return true;
        }

        //return false because there were no values to select from
        return false;
    }
    public List<DBType> GetAllAssets()
    {
        return _database.Values.ToList();
    }
}