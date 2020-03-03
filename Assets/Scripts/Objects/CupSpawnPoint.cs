using System;
using UnityEngine;

namespace Coffee
{
    public class CupSpawnPoint : MonoBehaviour, IInteractible
    {
        private enum States
        {
            CupSet,
            CupRemoved,
        }

        private States state;

        private Cup cup;

        public void Set(Cup c)
        {
            cup = c;
            
            var cupTf = cup.transform;
            var spawnTf = transform;
            
            cupTf.position = spawnTf.position;
            cupTf.rotation = spawnTf.rotation;
            
            cupTf.SetParent(spawnTf);

            state = States.CupSet;
        }

        public bool Use(IActor actor)
        {
            switch (state)
            {
                case States.CupSet:
                    var used = cup.Use(actor);
                    if (!used && actor.CurrentItem == null)
                    {
                        actor.GiveItem(cup);
                        cup = null;
                        state = States.CupRemoved;
                        return true;
                    }
                    return false;
                case States.CupRemoved:
                    if (actor.CurrentItem is Cup)
                    {
                        var c = (Cup)actor.RemoveItem();
                        Set(c);
                        return true;
                    }
                    break;
                default:
                    Debug.LogError("invalid state: " + state);
                    break;
            }

            return false;
        }
    }
}