using UnityEngine;
using Dan.Main;
using TMPro;

namespace UI
{
    public class LeaderboardsUI : UI
    {
        [Header("UI")]
        [SerializeField] 
        private Transform leaderboardContainer;
        [SerializeField] 
        private GameObject entryPrefab;

        [Header("Leaderboards")] 
        public int currentIndex;
        [SerializeField] 
        private TMP_Text currentTrack;
        [SerializeField] 
        private string[] trackAbbreviations;
        [SerializeField]
        private string[] publicKeys;
        
        // Start is called before the first frame update
        void Start()
        {
            GetLeaderboardByIndex(0);
        }

        // Update is called once per frame
        // void Update()
        // {
        //
        // }

        public void GetLeaderboardByIndex(int increment)
        {
            currentIndex = (currentIndex + increment) % publicKeys.Length;
            if (currentIndex < 0)
            {
                currentIndex += publicKeys.Length;
            }
            GetLeaderboard(publicKeys[currentIndex]);
            currentTrack.text = trackAbbreviations[currentIndex];
        }

        private void GetLeaderboard(string key)
        {
            LeaderboardCreator.GetLeaderboard(key, entries =>
            {
                if (leaderboardContainer.childCount < entries.Length)
                {
                    while (leaderboardContainer.childCount != entries.Length)
                    {
                        Instantiate(entryPrefab, leaderboardContainer);
                    }
                }

                for (var i = leaderboardContainer.childCount - 1; i >= 0; i--)
                {
                    if (i >= entries.Length)
                    {
                        Destroy(leaderboardContainer.GetChild(i).gameObject);
                        continue;
                    }

                    var entry = entries[i];
                    var entryField = leaderboardContainer.GetChild(i);

                    var textFields = entryField.GetComponentsInChildren<TMP_Text>();
                    textFields[0].text = (entries.Length - entry.Rank + 1).ToString() + '.';
                    textFields[1].text = entry.Username;

                    var times = entry.Extra.Split('/');
                    textFields[2].text = times[0];
                    textFields[3].text = times[1];
                }
            });
        }
    }
}
