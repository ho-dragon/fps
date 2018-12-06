public enum TeamCode {
    NONE = 0,
    RED = 1,
    BLUE = 2
}

public static class TeamTypeExtension {
    public static string GetTeamName(this TeamCode teamType) {
        switch (teamType) {
            case TeamCode.RED:
                return "Red";
            case TeamCode.BLUE:
                return "Blue";
        }
        return "";
    }
}