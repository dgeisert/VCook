using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Steamworks;

public class PlayerMachine : StateMachine {

	public static GameObject playerObject;
	public static PlayerMachine instance;
	public InputMachine inputMachine;
	public StateMaster stateMaster;
	private Dictionary<string, int> resources;
	private Dictionary<string, GameObject> loadedResources;
	protected Callback<GameOverlayActivated_t> Callback_GameOverlayActivated;
	protected Callback<LobbyCreated_t> Callback_lobbyCreated;
	protected Callback<LobbyMatchList_t> Callback_lobbyList;
	protected Callback<LobbyEnter_t> Callback_lobbyEnter;
	protected Callback<LobbyDataUpdate_t> Callback_lobbyInfo;
	protected Callback<GameLobbyJoinRequested_t> Callback_joinLobby;
	protected Callback<P2PSessionRequest_t> Callback_p2PSessionRequest;
	private List<CSteamID> ExpectingClient = new List<CSteamID>();
	private CSteamID lobbyID;

	public override void InstanceInitiate(StateMachine checkMachine){
		stateMaster.Setup ();
		inputMachine.Initiate ();
		PlayerMachine.playerObject = gameObject;
		PlayerMachine.instance = this;
		DontDestroyOnLoad (gameObject);
		if (ES2.Exists ("resources")) {
			resources = ES2.LoadDictionary<string, int> ("resources");
		} else {
			resources = new Dictionary<string, int> ();
		}
		Object[] objs = Resources.LoadAll ("");
		loadedResources = new Dictionary<string, GameObject> ();
		foreach (Object obj in objs) {
			if (obj.name.Substring (0, 3) == "obj") {
				loadedResources.Add (obj.name, (GameObject)obj);
			}
		}
		if (ES2.Exists ("playerLocation")) {
			Transform tr = ES2.Load<Transform> ("playerLocation");
			transform.position = tr.position;
			transform.localScale = tr.localScale;
			transform.localRotation = tr.localRotation;
			Destroy (tr.gameObject);
		} else {
			transform.position = AreaStartStateMachine.instance.transform.position;
			transform.rotation = AreaStartStateMachine.instance.transform.rotation;
		}
		LoadGos ();
		if(SteamManager.Initialized) {
			Callback_GameOverlayActivated = Callback<GameOverlayActivated_t>.Create(OnGameOverlayActivated);
			Callback_lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
			Callback_lobbyList = Callback<LobbyMatchList_t>.Create(OnLobbyList);
			Callback_lobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEnter);
			Callback_lobbyInfo = Callback<LobbyDataUpdate_t>.Create(OnLobbyInfo);
			Callback_joinLobby = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
			Callback_p2PSessionRequest = Callback<P2PSessionRequest_t>.Create(OnP2PSessionRequest);
			string name = SteamFriends.GetPersonaName();
			Debug.Log(name);
			if (name == "geisert" && isAtStartup) {
				Debug.Log ("Starting Server");
				SetupServer ();
			}
		}
	}

	void SendString(string str){
		byte[] bytes = new byte[str.Length * sizeof(char)];
		System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
		SendBytes (bytes);
	}

	public void SendBytes(byte[] bytes){
		foreach (CSteamID csid in ExpectingClient) {
			SteamNetworking.SendP2PPacket(csid, bytes, (uint) bytes.Length, EP2PSend.k_EP2PSendReliable);
		}
	}

	public void OnGameOverlayActivated(GameOverlayActivated_t overlay){

	}

	public void OnLobbyCreated(LobbyCreated_t lobbyCreated){
		lobbyID = CSteamID (lobbyCreated.m_ulSteamIDLobby);
		SteamFriends.ActivateGameOverlayInviteDialog (lobbyID);
		Debug.Log ("Lobby Created");
	}

	public void OnLobbyList(LobbyMatchList_t lobbyList){
		Debug.Log ("Lobby List");
	}

	public void OnLobbyEnter(LobbyEnter_t lobbyEnter){
		lobbyID = CSteamID (lobbyEnter.m_ulSteamIDLobby);
		int lobbyCount = SteamMatchmaking.GetNumLobbyMembers (lobbyID);
		for (int i = 0; i < lobbyCount; i++) {
			ExpectingClient.Add(SteamFriends.GetFriendFromSourceByIndex (lobbyID, i));
		}
		foreach (CSteamID csid in ExpectingClient) {
			Debug.Log (SteamFriends.GetFriendPersonaName (csid));
		}
	}

	public void OnLobbyInfo(LobbyDataUpdate_t lobbyInfo){
		Debug.Log ("Lobby Info");
	}

	public void OnJoinRequest(GameLobbyJoinRequested_t joinRequest){
		Debug.Log ("Requested to join lobby");
		JoinLobby (joinRequest.m_steamIDLobby);
	}

	void OnP2PSessionRequest(P2PSessionRequest_t request)
	{
		CSteamID clientId = request.m_steamIDRemote;
		if (ExpectingClient.Contains(clientId))
		{
			SteamNetworking.AcceptP2PSessionWithUser(clientId);
		} else {
			Debug.LogWarning("Unexpected session request from " + clientId);
		}
	}

	public override void InstanceUpdate(StateMachine checkMachine){
		if(Input.GetKeyDown(KeyCode.C)){
			SetupLocalClient ();
		}
	}

	NetworkClient myClient;
	bool isAtStartup = true;
	// Create a server and listen on a port
	public void SetupServer()
	{
		if (SteamManager.Initialized) {
			CreateLobby (ELobbyType.k_ELobbyTypeFriendsOnly, 2);
		}
	}

	SteamAPICall_t CreateLobby (ELobbyType eLobbyType, int maxMembers){
		return SteamMatchmaking.CreateLobby (eLobbyType, maxMembers);
	}

	SteamAPICall_t JoinLobby (CSteamID lobby){
		return SteamMatchmaking.JoinLobby (lobby);
	}

	bool InviteUserToLobby(CSteamID lobby, CSteamID invitee){
		return false;
	}

	// Create a client and connect to the server port
	public void SetupClient()
	{
		myClient = new NetworkClient();
		myClient.RegisterHandler(MsgType.Connect, OnConnected);
		myClient.Connect("127.0.0.1", 4444);
		isAtStartup = false;
	}

	// Create a local client and connect to the local server
	public void SetupLocalClient()
	{
		myClient = ClientScene.ConnectLocalServer();
		myClient.RegisterHandler(MsgType.Connect, OnConnected);     
		isAtStartup = false;
	}

	public void OnConnected(NetworkMessage nmsg){
		Debug.Log (nmsg.ToString());
	}

	public int GetResource(string resource){
		resource = resource.ToLower ();
		if(!resources.ContainsKey(resource)) {
			CreateResource (resource);
		}
		return resources [resource];
	}
	private void CreateResource(string resource){
		resources.Add (resource, 0);
	}
	public void SetResource(string resource, int count){
		resource = resource.ToLower ();
		if(!resources.ContainsKey(resource)) {
			CreateResource (resource);
		}
		resources [resource] = count;
		SaveResources ();
		SaveGos ();
	}
	public void AddResource(string resource, int increment){
		resource = resource.ToLower ();
		if(!resources.ContainsKey(resource)) {
			CreateResource (resource);
		}
		resources [resource] += increment;
		SaveResources ();
		SaveGos ();
	}
	public void SavePlayerPosition(){
		ES2.Save (transform, "playerLocation");
	}
	public void SaveResources(){
		SavePlayerPosition ();
		ES2.Save(resources, "resources");
	}
	public void SaveGos(){
		InputMachine.instance.gos.RemoveAll (item => item == null);
		Dictionary<Transform, SaveObject> saveGos = new Dictionary<Transform, SaveObject> ();
		foreach (GameObject go in InputMachine.instance.gos) {
			StateMachine sm = go.GetComponent<StateMachine> ();
			saveGos.Add (go.transform, new SaveObject(sm));
		}
		ES2.Save(saveGos, "gos");
	}
	public void LoadGos(){
		if (InputMachine.instance.gos == null) {
			InputMachine.instance.gos = new List<GameObject> ();
		}
		InputMachine.instance.gos.RemoveAll (item => item == null);
		if (ES2.Exists ("gos")) {
			Dictionary<Transform, SaveObject> loadGos = ES2.LoadDictionary<Transform, SaveObject> ("gos");
			foreach (KeyValuePair<Transform, SaveObject> kvp in loadGos) {
				if (loadedResources.ContainsKey (kvp.Value.objName)) {
					GameObject go = (GameObject)GameObject.Instantiate (loadedResources [kvp.Value.objName]);
					go.GetComponent<StateMachine> ().Load(kvp.Key, kvp.Value);
					Destroy (kvp.Key.gameObject);
				}
			}
			InputMachine.instance.CheckObjects ();
		}
	}
}
