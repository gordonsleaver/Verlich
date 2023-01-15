
using UnityEngine.Networking;
public class CertificateBypass : CertificateHandler
{
    /// <summary>
    /// Overwrites any certificate requirement
    /// </summary>
    /// <param name="certificateData"></param>
    /// <returns></returns>
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        //Simply return true no matter what
        return true;
    }
}
