using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiController : MonoBehaviour {
	public ContractController contractController;

	public GameObject[] UiPanelList;
	public const int PANEL_CREATEUSER = 0;
	public const int PANEL_UPDATEPING = 1;

	// Use this for initialization
	void Start () {
		PlayerPrefs.DeleteAll ();
		contractController.LoadUser ();
		if (contractController.userAddress == null || contractController.userAddress == "") {
			ActivateCreateUserPanel ();
		} else {
			ActivateUpdatePingPanel ();
		}
	}

	public void ActivateCreateUserPanel() {
		UiPanelList [PANEL_CREATEUSER].SetActive (true);
		UiPanelList [PANEL_UPDATEPING].SetActive (false);
	}

	public void ActivateUpdatePingPanel() {
		UiPanelList [PANEL_CREATEUSER].SetActive (false);
		UiPanelList [PANEL_UPDATEPING].SetActive (true);
	}
}
