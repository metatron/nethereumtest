using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Threading.Tasks;

using Nethereum.Web3;

using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;


public class NethereumTest : MonoBehaviour {

	// Use this for initialization
	async void Start () {
//		ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback((sender, certificate, chain, policyErrors) => { return true; });

		ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
//		ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
//		ServicePointManager.SecurityProtocol = (SecurityProtocolType)192 | (SecurityProtocolType)768 | (SecurityProtocolType)3072;
//		System.Net.ServicePointManager.SecurityProtocol &= ~SecurityProtocolType.Ssl3;

			



		Web3 web3 = new Web3("https://ropsten.infura.io/hr1s0JoyZSF1c0aA2FoT");
		var balance = await web3.Eth.GetBalance.SendRequestAsync("0x8dc649c02DD290410BDFe17aDaB1e21E92167Ef8");

		Debug.Log ("balance: " + balance);

	}




	private static bool RemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
	{
		//Return true if the server certificate is ok
		if (sslPolicyErrors == SslPolicyErrors.None)
			return true;

		bool acceptCertificate = true;
		string msg = "The server could not be validated for the following reason(s):\r\n";

		//The server did not present a certificate
		if ((sslPolicyErrors &
			SslPolicyErrors.RemoteCertificateNotAvailable) == SslPolicyErrors.RemoteCertificateNotAvailable)
		{
			msg = msg + "\r\n    -The server did not present a certificate.\r\n";
			acceptCertificate = false;
		}
		else
		{
			//The certificate does not match the server name
			if ((sslPolicyErrors &
				SslPolicyErrors.RemoteCertificateNameMismatch) == SslPolicyErrors.RemoteCertificateNameMismatch)
			{
				msg = msg + "\r\n    -The certificate name does not match the authenticated name.\r\n";
				acceptCertificate = false;
			}

			//There is some other problem with the certificate
			if ((sslPolicyErrors &
				SslPolicyErrors.RemoteCertificateChainErrors) == SslPolicyErrors.RemoteCertificateChainErrors)
			{
				foreach (X509ChainStatus item in chain.ChainStatus)
				{
					if (item.Status != X509ChainStatusFlags.RevocationStatusUnknown &&
						item.Status != X509ChainStatusFlags.OfflineRevocation)
						break;

					if (item.Status != X509ChainStatusFlags.NoError)
					{
						msg = msg + "\r\n    -" + item.StatusInformation;
						acceptCertificate = false;
					}
				}
			}
		}

		//If Validation failed, present message box
		if (acceptCertificate == false)
		{
			msg = msg + "\r\nDo you wish to override the security check?";
			//          if (MessageBox.Show(msg, "Security Alert: Server could not be validated",
			//                       MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
			acceptCertificate = true;
		}

		return acceptCertificate;
	}

	
	// Update is called once per frame
	void Update () {
		
	}
}
