using CBS.Context;
using PlayFab.Internal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppContext : SingletonMonoBehaviour<AppContext>
{
    public GameContext game;

    public FirebaseDatabaseManager DB { get; internal set; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void Initialize()
    {
        
    }
}
