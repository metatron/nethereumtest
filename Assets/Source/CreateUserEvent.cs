using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateUserEvent : MonoBehaviour {
	public ContractController contractController;
	public UiController uiController;

	public UnityEngine.UI.InputField passwordField;

	public async void OnCreateUserButton() {
		await contractController.CreateUser (passwordField.text);
		uiController.ActivateUpdatePingPanel ();

		await contractController.GetBalance (contractController.userAddress);
	}

	public async void OnLoadUserByPrivateKeyButton() {
		Debug.LogError ("input address: " + passwordField.text);
		await contractController.LoadUserByPrivateKey (passwordField.text);
		uiController.ActivateUpdatePingPanel ();

		await contractController.GetBalance (contractController.userAddress);
	}
}
