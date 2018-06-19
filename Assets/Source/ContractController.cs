using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Nethereum;
using Nethereum.Geth;
using Nethereum.Contracts;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Web3;
using Nethereum.Web3.Accounts.Managed;
using Nethereum.RPC.Eth;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Hex;


using Nethereum.Signer;
using Nethereum.Signer.Crypto;

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ContractController : MonoBehaviour {

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

	private string contractAddress = "0x0b0a24c1a001ad78dc43cccea4f3cf6c83c5676f";

	private Web3 web3;
	private Contract contract;
	public string userAddress;
	public string privateKey;

	//	private string privateKey = "ab5aef177d8e317bea7d76cbf5ac083e8be617891ff2089c4573cf63a15b9465";
	private string url = "http://localhost:8545";

	public const string USER_ADDRESS	= "userAddress";
	public const string USER_PRIVATEKEY	= "privateKey";


	// Use this for initialization
	async void Start () {
		this.web3 = new Web3(url);
//		this.address = EthECKey.GetPublicAddress(privateKey); //could do checksum
		this.contract = web3.Eth.GetContract(ABI, contractAddress);

//		string privateKey = "ab5aef177d8e317bea7d76cbf5ac083e8be617891ff2089c4573cf63a15b9465";
//		string address = EthECKey.GetPublicAddress(privateKey); //could do checksum
//		await GetBalance(address);

		GetPing ();
	}

	public void LoadUser() {
		privateKey = PlayerPrefs.GetString (USER_PRIVATEKEY);
		userAddress = PlayerPrefs.GetString (USER_ADDRESS);

	}


	public async Task CreateUser(string password) {
//		EthECKey ecKey = Nethereum.Signer.EthECKey.GenerateKey();
//		byte[] bytePKey = ecKey.GetPrivateKeyAsBytes ();
//		Nethereum.Web3.Accounts.Account account = new Nethereum.Web3.Accounts.Account (bytePKey);

		userAddress = await web3.Personal.NewAccount.SendRequestAsync(password);


		Debug.Log ("account created: " + userAddress);

//		privateKey = System.Text.Encoding.GetEncoding("ISO-8859-1").GetString(bytePKey);
//		PlayerPrefs.SetString (USER_PRIVATEKEY, privateKey);

		PlayerPrefs.SetString (USER_ADDRESS, userAddress);

		var unlockAccountResult =
			await web3.Personal.UnlockAccount.SendRequestAsync(userAddress, password, new Nethereum.Hex.HexTypes.HexBigInteger(120));

		Debug.Log ("unlockAccountResult: " + unlockAccountResult);
	}

	public async Task LoadUserByPrivateKey(string privateKey) {
		userAddress = EthECKey.GetPublicAddress (privateKey);
		Debug.Log ("account loaded: " + userAddress);
		PlayerPrefs.SetString (USER_ADDRESS, userAddress);
	}


	public async void GetPing() {
//		this.address = EthECKey.GetPublicAddress(privateKey); //could do checksum
		this.contract = web3.Eth.GetContract(ABI, contractAddress);

		int ping = await contract.GetFunction("getPing").CallAsync<int>().ConfigureAwait(true);
		Debug.Log ("ping: " + ping);
	}



	public async Task MineAndGetReceiptAsync(ulong ping){

		var setPingFunction = contract.GetFunction("setPint");
		Nethereum.Contracts.Event pongEvent = contract.GetEvent("Pong");

		Nethereum.Hex.HexTypes.HexBigInteger estimatedGas = await setPingFunction.EstimateGasAsync (ping);


		Nethereum.RPC.Eth.DTOs.TransactionReceipt receipt =  await setPingFunction.SendTransactionAndWaitForReceiptAsync(userAddress, estimatedGas, null, null, ping);
		Debug.Log ("receipt: " + receipt.TransactionHash + ", blockNum: " + receipt.BlockNumber.Value);


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
	}

	public class PongEvent
	{
		[Parameter("int", "updated", 1, true)]
		public int Pong {get; set;}
	}


	public async Task TransferEth(string addressTo, ulong amountBigInt) {
		Debug.Log ("TransferEth amountBigInt: " + amountBigInt);
		var transactionReceipt = await web3.TransactionManager.TransactionReceiptService.SendRequestAndWaitForReceiptAsync(() =>
			web3.TransactionManager.SendTransactionAsync(userAddress, addressTo, new Nethereum.Hex.HexTypes.HexBigInteger(amountBigInt))
		);

		Debug.Log (transactionReceipt);
	}

	public async Task<string> GetBalance(string address) {
		Nethereum.Hex.HexTypes.HexBigInteger balance = await web3.Eth.GetBalance.SendRequestAsync(address); 

		Debug.Log ("address: " + address + ", Balance: " + balance.Value.ToString());

		return balance.Value.ToString ();
	}

}
