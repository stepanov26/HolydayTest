using System;
using System.Collections.Generic;
using UnityEngine;

public class Achievements {

    public const int bananaGoldIncreasePerMilestone = 1;
    
    public void AddProgress(Achievement achievement, int extraProgress)
    {
        AchievementsValues[achievement] += extraProgress;

        if (achievement == Achievement.BananaUpgrades)
        {
            return;
        }
        
        GameEntities.SocketConnection.AddAchievementProgress(achievement, extraProgress);
    }

    public int GetTotalAchievementsEffect()
    {
        var result = 0;

        foreach (var achievement in AchievementsValues.Keys)
        {
            result += GetAchievementEffect(achievement);
        }
        
        return result;
    }

    public int GetAchievementEffect(Achievement achievement)
    {
        return (GetCurrentMilestoneIndex(achievement) + 1) * bananaGoldIncreasePerMilestone;
    }

    public int GetCurrentMilestoneIndex(Achievement achievement)
    {
        var achievementEffect = 0;
        
        switch (achievement)
        {
            case Achievement.BananasClicked:
                achievementEffect = bananaClicks.FindLastIndex(milestone => AchievementsValues[achievement] >= milestone);
                break;
            case Achievement.BananaUpgrades:
                achievementEffect = bananaUpgrades.FindLastIndex(milestone => AchievementsValues[achievement] >= milestone);
                break;
            default:
                Debug.LogError("Unexpected achievement type");
                break;
        }
        
        return achievementEffect;
    }

    private static readonly List<int> bananaClicks = new List<int>()
    {
        10,
        20,
        30,
        40,
        50,
        60,
        70,
        80,
        90,
        100,
        150,
        200,
        250,
        300,
        350,
        400,
        450,
        500,
        600,
        700,
        800,
        900,
        1000,
        1500,
        2000,
        2500,
        3000,
        3500,
        4000,
        4500,
        5000,
        6000,
        7000,
        8000,
        9000,
        10000,
    };

    private static readonly List<int> bananaUpgrades = new List<int>()
    {
        1,
        3,
        5,
        8,
        11,
        14,
        17,
        20,
        23,
        26,
        29,
        32,
        36,
        40,
        44,
        48,
        52,
        56,
        60,
        64,
        68,
        72,
    };

    public static readonly Dictionary<Achievement, List<int>> achievementMilestones = new Dictionary<Achievement, List<int>>() {
        { Achievement.BananasClicked,       bananaClicks },
        { Achievement.BananaUpgrades,       bananaUpgrades },
    };

    public readonly Dictionary<Achievement, int> AchievementsValues = new Dictionary<Achievement, int>()
    {
        { Achievement.BananasClicked, 0 },
        { Achievement.BananaUpgrades, 0 },
    };
}

public enum Achievement { 
    BananasClicked,
    BananaUpgrades
}

[Serializable]
public class AchievementServerList {
    public List<AchievementServer> list = new List<AchievementServer>();
}

[Serializable]
public class AchievementServer {
    public Achievement code;
    public int number;
}