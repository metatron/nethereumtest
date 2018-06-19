using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Threading.Tasks;


public class UpdatePing : MonoBehaviour {
	public ContractController contractController;
	public UnityEngine.UI.InputField amountField;
	public UnityEngine.UI.InputField toAddressField;

	public async void OnUpdatePingButton() {
		ulong data = System.Convert.ToUInt64 (amountField.text);
		await contractController.MineAndGetReceiptAsync(data);
	}

	public async void OnTransferEth() {
		ulong data = System.Convert.ToUInt64 (amountField.text);
		Debug.Log ("OnTransferEth.data: " + data);
		await contractController.TransferEth (toAddressField.text, data);

		string balance = await contractController.GetBalance (contractController.userAddress);
		Debug.Log ("from: " + balance);
		balance = await contractController.GetBalance (toAddressField.text);
		Debug.Log ("to: " + balance);
	}
}
