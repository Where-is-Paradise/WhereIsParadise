using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CertifcateValidator : CertificateHandler
{
    // Start is called before the first frame update
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        return true;
    }
}
