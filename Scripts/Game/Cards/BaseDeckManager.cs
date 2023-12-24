using Godot;
using System;
using System.Collections.Generic;

public abstract class BaseDeckManager
{
    public BaseDeckModel Model { get; private set; }

    public BaseDeckManager(BaseDeckModel deckInfo) 
    {
        Model = deckInfo;
    }

    public abstract BaseCardWrapper[] GetBaseDeck();
}
