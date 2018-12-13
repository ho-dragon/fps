using UnityEngine;
using UnityEngine.Assertions;

public class MapInfo : MonoBehaviourInstance<MapInfo> {
    public Transform redZone;
    public Transform blueZone;
    public Transform waitingZone;

    private void Awake() {
        Assert.IsNotNull(this.redZone);
        Assert.IsNotNull(this.blueZone);
        Assert.IsNotNull(this.waitingZone);
    }

    public Vector3 GetRespawnZone(TeamCode teamCode) {
        switch (teamCode) {
            case TeamCode.BLUE:
                return GetBlueZone();
            case TeamCode.RED:
                return GetRedZone();
        }
        return GetWaitingZone();
    }

    private Vector3 GetRedZone() {
        return this.redZone.position + new Vector3(UnityEngine.Random.Range(-3f, 3f), UnityEngine.Random.Range(-3f, 3f), 0);
    }

    private Vector3 GetBlueZone() {
        return this.blueZone.position + new Vector3(UnityEngine.Random.Range(-3f, 3f), UnityEngine.Random.Range(-3f, 3f), 0); ;
    }

    private Vector3 GetWaitingZone() {
        return this.waitingZone.position + new Vector3(UnityEngine.Random.Range(-3f, 3f), UnityEngine.Random.Range(-3f, 3f), 0); ;
    }

    public void EnableZone(bool isRunningGame) {
        this.waitingZone.gameObject.SetActive(!isRunningGame);
        this.redZone.gameObject.SetActive(isRunningGame);
        this.blueZone.gameObject.SetActive(isRunningGame);
    }
}
