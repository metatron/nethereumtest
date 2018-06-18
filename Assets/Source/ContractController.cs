using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Nethereum;
using Nethereum.Geth;
using Nethereum.Contracts;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Web3;
using Nethereum.Web3.Accounts.Managed;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Hex;


using Nethereum.Signer;
using Nethereum.Signer.Crypto;

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ContractController : MonoBehaviour {

	private Web3 web3;
	private string address;
	private Contract contract;

	private static string ABI = @"[
    {
      ""constant"": true,
      ""inputs"": [],
      ""name"": ""ping"",
      ""outputs"": [
        {
          ""name"": """",
          ""type"": ""uint256""
        }
      ],
      ""payable"": false,
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""anonymous"": false,
      ""inputs"": [
        {
          ""indexed"": true,
          ""name"": ""updated"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""Pong"",
      ""type"": ""event""
    },
    {
      ""constant"": true,
      ""inputs"": [],
      ""name"": ""getPing"",
      ""outputs"": [
        {
          ""name"": """",
          ""type"": ""uint256""
        }
      ],
      ""payable"": false,
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""constant"": false,
      ""inputs"": [
        {
          ""name"": ""newPing"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""setPint"",
      ""outputs"": [],
      ""payable"": false,
      ""stateMutability"": ""nonpayable"",
      ""type"": ""function""
    }
  ]";

	private string contractAddress = "0xbca3abeb004062e38f83068409204c48870a69bd";

	private string privateKey = "b3c24a1e15c21620b68d919b447ed8c2c3afe778c2f9f3308504bcad81054f50";
	private string url = "http://localhost:8545";

	private Nethereum.Web3.Accounts.Account account;

	// Use this for initialization
	async void Start () {
		this.web3 = new Web3(url);
		this.address = EthECKey.GetPublicAddress(privateKey); //could do checksum

		this.contract = web3.Eth.GetContract(ABI, contractAddress);

		// call sample
//		int ping = await contract.GetFunction("getPing").CallAsync<int>().ConfigureAwait(true);
//		Debug.Log ("ping: " + ping);

//		await MineAndGetReceiptAsync ();
	}


	async public void CreateUser() {
		EthECKey ecKey = Nethereum.Signer.EthECKey.GenerateKey();
		var privateKey = ecKey.GetPrivateKeyAsBytes ();
		account = new Nethereum.Web3.Accounts.Account (privateKey);
		Debug.Log ("account created: " + account.Address);
	}

}
