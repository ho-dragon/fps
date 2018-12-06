using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager {
    public delegate void UpdatePlayTimeHandler(int PlayTime);
    public event UpdatePlayTimeHandler OnUpdatePlayTime;
    public void UpdatePlayTime(int PlayTime) {
        if (OnUpdatePlayTime != null) {
            OnUpdatePlayTime(PlayTime);
        }
    }

}
