using UnityEngine;
using System.Collections.Generic;

public class GameContextModel {
    public Dictionary<int, int> playerTeamNumbers;
    public int remainTimeToEnd;
    public int scoreRed;
    public int scoreBlue;
}

public enum TeamType {
    NONE = 0,
    RED = 1,
    BLUE = 2
}

public static class TeamTypeExtension {
    public static string GetTeamName(this TeamType teamType) {
        switch (teamType) {
            case TeamType.RED:
                return "Red";
            case TeamType.BLUE:
                return "Blue";
        }
        return "";
    }
}