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
//using System.Numerics;


public class TestWeb3 : MonoBehaviour {

	public string PrivateKey { get; private set; }
	public string Url { get; private set; }
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

	// Use this for initialization
	async void Start () {
		this.PrivateKey = "ab5aef177d8e317bea7d76cbf5ac083e8be617891ff2089c4573cf63a15b9465";
		this.Url = "http://localhost:8545";
		string contractAddress = "0x0b0a24c1a001ad78dc43cccea4f3cf6c83c5676f";


//		var account = new ManagedAccount(senderAddress, password);

		this.web3 = new Web3(Url);
		this.address = EthECKey.GetPublicAddress(this.PrivateKey); //could do checksum

		this.contract = web3.Eth.GetContract(TestWeb3.ABI, contractAddress);

		// call sample
		int ping = await contract.GetFunction("getPing").CallAsync<int>().ConfigureAwait(true);
		Debug.Log ("ping: " + ping);


		Nethereum.Hex.HexTypes.HexBigInteger balance = await web3.Eth.GetBalance.SendRequestAsync(address); 
		Debug.Log ("Balance: " + balance.Value.ToString());


		await MineAndGetReceiptAsync ();
	}


	public async Task MineAndGetReceiptAsync(){

		var setPingFunction = contract.GetFunction("setPint");
		Nethereum.Contracts.Event pongEvent = contract.GetEvent("Pong");

//		var filterAll = await pongEvent.CreateFilterAsync ();
//		Debug.Log ("filterAll: " + filterAll);
//		var filter7 = await pongEvent.CreateFilterAsync(7);

		Nethereum.Hex.HexTypes.HexBigInteger estimatedGas = await setPingFunction.EstimateGasAsync (111);


//		var transactionHash = await setPingFunction.SendTransactionAsync(address, estimatedGas, null,  8);
//		Debug.Log ("transactionHash: " + transactionHash);
		Nethereum.RPC.Eth.DTOs.TransactionReceipt receipt =  await setPingFunction.SendTransactionAndWaitForReceiptAsync(address, estimatedGas, null, null, 111);
		Debug.Log ("receipt: " + receipt.TransactionHash + ", blockNum: " + receipt.BlockNumber.Value);


//		object[] array = new[] { "9" };
		var filterInput = pongEvent.CreateFilterInput(new BlockParameter(0), BlockParameter.CreateLatest());
		var logs = await pongEvent.GetAllChanges<PongEvent>(filterInput);

		foreach(Nethereum.Contracts.EventLog<PongEvent> log in logs) {
			Debug.Log("================================");
			Debug.Log("address: " + log.Log.Address);
			Debug.Log("TransactionHash: " + log.Log.TransactionHash);
			Debug.Log("blockNum: " + log.Log.BlockNumber);
			Debug.Log("data: " + log.Log.Data);
			Debug.Log("Pong: " + log.Event.Pong);
		}


//		var log = await pongEvent.GetFilterChanges<PongEvent>(filterAll);
//		var log7 = await pongEvent.GetFilterChanges<PongEvent>(filter7);

		Debug.Log("logs: " + logs.Count);
//		Debug.Log("log: " + log.Count);

	}

	//event Pong(uint256 pong);


	public class PongEvent
	{
		[Parameter("int", "updated", 1, true)]
		public int Pong {get; set;}
	}

}
