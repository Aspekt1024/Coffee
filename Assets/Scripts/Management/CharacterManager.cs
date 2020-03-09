using Coffee.Characters;
using UnityEngine;

namespace Coffee
{
    public class CharacterManager : IManager
    {
        private Transform spawnPoint;

        public Actor Player { get; private set; }

        public void Init()
        {
            Player = Object.FindObjectOfType<Actor>();
            spawnPoint = Object.FindObjectOfType<SpawnPoint>().transform;
        }

        public void ResetActor()
        {
            var playerPos = spawnPoint.position;
            playerPos.y = Player.transform.position.y;
            Player.transform.position = playerPos;

            Player.ResetState();
        }
    }
}