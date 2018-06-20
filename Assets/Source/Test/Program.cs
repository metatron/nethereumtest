using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts.CQS;
using Nethereum.Util;
using Nethereum.Web3.Accounts;

using UnityEngine;
using Nethereum.Web3;



	public class Program : MonoBehaviour {

	async void Start()  {
			//To connect to infura using .net451 and TSL2 you need to set it in advance
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
		System.Net.ServicePointManager.Expect100Continue = false;

			// The rest is the same as other versions of the framework

//			Console.WriteLine("Deploying the contract");
//			// Remove comment to deploy a new contract
//			//DeployStandardTokenAsync().Wait();
//			Console.ReadLine();
//			Console.WriteLine("Checking the balance");
//			BalanceAsync().Wait();
//			Console.ReadLine();
//			Console.WriteLine("Transfering...");
//			TransferAsync().Wait();
//			Console.ReadLine();

		await BalanceAsync();
	}

		//This is the contract address of an already deployed smartcontract in the Rinkey Test Chain
		private static string ContractAddress { get; set; } = "0xe757820308b5701302341cecd8a62c8d425c782b";

		public static async Task BalanceAsync()
		{
			//Replace with your own
			var senderAddress = "0x12890d2cce102216644c59daE5baed380d84830c";
			var contractAddress = ContractAddress;
			var url = "https://rinkeby.infura.io/";
			//no private key we are not signing anything
			var web3 = new Web3(url);

			var balanceOfFunctionMessage = new BalanceOfFunction()
			{
				Owner = senderAddress,
			};

			var balanceHandler = web3.Eth.GetContractQueryHandler<BalanceOfFunction>();
			var balance = await balanceHandler.QueryAsync<BigInteger>(balanceOfFunctionMessage, contractAddress);


			Console.WriteLine("Balance of token: " + balance);

		}

		[Function("balanceOf", "uint256")]
		public class BalanceOfFunction : ContractMessage
		{

			[Parameter("address", "_owner", 1)]
			public string Owner { get; set; }

		}

		public static async Task TransferAsync()
		{
			//Replace with your own
			var senderAddress = "0x12890d2cce102216644c59daE5baed380d84830c";
			var receiverAddress = "0xde0B295669a9FD93d5F28D9Ec85E40f4cb697BAe";
			var privatekey = "0xb5b1870957d373ef0eeffecc6e4812c0fd08f554b37b233526acc331bf1544f7";
			var url = "https://rinkeby.infura.io/";

			var web3 = new Web3(new Account(privatekey), url);

			var transactionMessage = new TransferFunction()
			{
				FromAddress = senderAddress,
				To = receiverAddress,
				TokenAmount = 100,
				//Set our own price
				GasPrice = Web3.Convert.ToWei(25, UnitConversion.EthUnit.Gwei)

			};



			var transferHandler = web3.Eth.GetContractTransactionHandler<TransferFunction>();

			/// this is done automatically so is not needed.
			var estimate = await transferHandler.EstimateGasAsync(transactionMessage, ContractAddress);
			transactionMessage.Gas = estimate.Value;


			var transactionHash = await transferHandler.SendRequestAsync(transactionMessage, ContractAddress);
			Console.WriteLine(transactionHash);

		}


		[Function("transfer", "bool")]
		public class TransferFunction : ContractMessage
		{
			[Parameter("address", "_to", 1)]
			public string To { get; set; }

			[Parameter("uint256", "_value", 2)]
			public BigInteger TokenAmount { get; set; }
		}
	}