using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PoolingTester : MonoBehaviourPun
{
    private PhotonView pv;
    public GameObject testPrefab;
    public GameObject testObj;
    public List<GameObject> testObjList;    // 현재 활성화되어있는 testObj 관리

    // * 활성화/비활성화 상관 없이 Create 된 testObj 전부 관리 위한다면 게임 초기
    // * 사이즈를 임의로 정하여 PoolContainer 미리 선언해두는게 좋음
    // * 만약 필요 사이즈가 커진다면 그 때 동적으로 Container 내부 오브젝트 추가 생성
    // ** 다른 방법 => 총알과 같이 많은 오브젝트 생성의 경우 -> 가장 초기에 생성된 오브젝트를 그 위치로 변경, 초기 오브젝트를 삭제한 것처럼 보이게 할 수 있음  

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    private void Start()
    {
        testObjList = new List<GameObject>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            pv.RPC("RequestObjectCreation", RpcTarget.MasterClient);
        }
    }

    // ? >> 만약 Request 없이 곧바로 Target.All 로 모두가 생성 시킨다면 생길 문제점은?
    [PunRPC]
    private void RequestObjectCreation()
    {
        Vector3 spawnPosition = transform.position; // 생성 위치 (원하는 위치로 설정 가능)
        Quaternion spawnRotation = transform.rotation;

        // 마스터가 오브젝트 생성 명령을 모든 클라이언트에 전달
        photonView.RPC("CreateSharedObjectRPC", RpcTarget.All, spawnPosition, spawnRotation);
    }

    [PunRPC]
    private void CreateSharedObjectRPC(Vector3 position, Quaternion rotation)
    {
        // 각 클라이언트는 로컬 풀에서 오브젝트를 가져와 지정된 위치에 생성
        testObj = GameManager.Pool.Get(testPrefab, position, rotation);
        
        if (testObj != null)
        {
            testObj.SetActive(true);
            testObjList.Add(testObj);
        }
    }

    public void RemoveTestObject(int index)
    {
        photonView.RPC("RequestObjectRemove", RpcTarget.MasterClient, index);
    }

    [PunRPC]
    public void RequestObjectRemove(int objIndex)
    {
        if(testObjList[objIndex] != null)
        {
            GameObject obj = testObjList[objIndex];

            if(obj != null)
                photonView.RPC("RemoveTestObjectRPC", RpcTarget.All, objIndex);
        }
    }

    [PunRPC]
    private void RemoveTestObjectRPC(int objIndex)
    {
        GameManager.Pool.Release(testObjList[objIndex]);
        testObjList.RemoveAt(objIndex);
    }
}
