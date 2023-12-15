using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class UserInfoResponse
{

    public UserInfoResult response;

}


[System.Serializable]
public class UserInfoResult
{
    public string result;
    public UserInfoParam Params;
}



[System.Serializable]
public class UserInfoParam
{
    public string state;
    public string country;
    public string currency;
    public string status;
}