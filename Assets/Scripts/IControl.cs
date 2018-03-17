using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class IControl : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
	public Vector2 joypad
	{
		get
		{
			Vector2 input = Vector2.zero;
			if (drag) {
				Vector2 pos = joyStick.anchoredPosition;
				input = pos / (joyPanel.sizeDelta.y / 2);
			}
			return input;
		}
	}
	public static IControl control;
	public RectTransform joyPanel;
	public RectTransform joyStick;
	public static Transform cam;
	public static Camera camMain;
	public RectTransform healthBar;

	public static ICharacter character;

	public RectTransform slotsParent;
	public RectTransform slotsMask;
	//public Scrollbar inventoryScroll;
	public int[] currentItems;
	public int currentLookableItemIndex = -1;
	private int probeIndexLookable = 0;
	public Text itemsInfo;
	public GameObject inventoryFunctionsLabel;

	public GameObject runtimeObj;
	public GameObject inventoryObj;
	public GameObject pauseObj;
	public GameObject dialogObj;
	public GameObject deathMenuObj;
	public GameObject runeMenuObj;
	public GameObject characterMenuObj;
	public GameObject lootMenuObj;

	public Button toInventory;
	public Button toGame;
	Image toGameImage;
	public Button toGameMenu;
	public Button toCharacterMenu;
	public Button saveAndToMenu;

	public Button drop;
	public Button use;

	public Button amuletInv;
	public Button armorInv;
	public Button weapontInv;
	public Button runeInv;

	public RectTransform parent;
	public Text answer;

	public Button runtimeUse;
	Image rui;
	public Button runtimeTalk;
	public IDragButton runtimeAttack;

	public RawImage spellButton;

	[Header("Rune create menu")]
	public Button rune_fire;
	public Button rune_water;
	public Button rune_earth;
	public Button rune_air;
	public Button rune_god;

	public GameObject runeStatsParent;
	public GameObject armorStatsParent;

	public RawImage[] runeStats;
	public RawImage[] armorStats;

	public RawImage portait;
	public RawImage dialogPortait;

	public RectTransform lootSlotsParent;
	public RectTransform lootMineSlotsParent;

	public Text characterInfoText;

	public float deathTime = 0;

	public RectTransform messagesParent;

	public enum IGameMenuState
	{
		Runtime,
		Pause,
		Inventory,
		Dialog,
		Death,
		Screenshot,
		RuneCreate,
		CharacterInfo,
		Loot
	}

	public IGameMenuState state;

	public void SetState () {
		runtimeObj.SetActive (state == IGameMenuState.Runtime);
		inventoryObj.SetActive (state == IGameMenuState.Inventory);
		pauseObj.SetActive (state == IGameMenuState.Pause);
		dialogObj.SetActive (state == IGameMenuState.Dialog);
		deathMenuObj.SetActive (state == IGameMenuState.Death);
		runeMenuObj.SetActive (state == IGameMenuState.RuneCreate);
		characterMenuObj.SetActive (state == IGameMenuState.CharacterInfo);
		lootMenuObj.SetActive (state == IGameMenuState.Loot);

		if (state != IGameMenuState.Loot && lootableChest) {
			lootableChest = null;
		}

		toGame.gameObject.SetActive (state != IGameMenuState.RuneCreate);

		if (state != IGameMenuState.Runtime) {
			Time.timeScale = 0;
		} else {
			Time.timeScale = 1;
		}

		IFontSetter.SetFontForall ();
	}

	public void OutDialog () {
		state = IGameMenuState.Runtime;
		SetState ();
	}

	public void ToDialog () {
		ICharacter with = character.canTalk;
		dialog_window.with = with;
		dialog_window.for_answer = answer;
		dialog_window.parent = parent;
		dialog_window.control = this;
		dialogPortait.texture = ITextureDrawer.GetFromPerson (with.status);
		CameraMotor ();
		dialog_window.InitializeNode ();

		//dialog_window.PrepareStage ();
	}
	public IDialogWindow dialog_window;

	private bool drag = false;
	bool probe = true;
	public bool characterDead = false;

	private void Update () {
		if (Time.time - deathTime > 5 && characterDead) {
			if (state != IGameMenuState.Death) {
				state = IGameMenuState.Death;
				SetState ();
			}
		}
		if (!character) {
			character = ICharacter.GetPlayer();
			if (!character && !characterDead) {
				characterDead = true;
				runtimeObj.SetActive (false);
				deathTime = Time.time;
			}
		}
		JoySinhro ();
		if (character) {
			IndicatorUpdate ();
			InterfaceMotor();
		}
	}
	private void IndicatorUpdate () {
		healthBar.SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, 60 + (((float)character.status.health / (float)character.status.maxHealth) * 100) * 2);
	}
	private void FixedUpdate () {
		if (character) {
			CameraMotor ();
		}
	}
	public static void SetTextWithScales (Text t, string msg) {
		RectTransform trans = t.rectTransform;
		TextGenerationSettings s = t.GetGenerationSettings (trans.rect.size);
		float slotSize = t.cachedTextGenerator.GetPreferredHeight(msg, s);
		trans.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, slotSize);
		t.text = msg;
	}
	private void FillMessages (RectTransform parent, DebugMessage[] msg) {
		GameObject prefab = (GameObject)Resources.Load ("Prefabs/Message");
		Text[] texts = parent.GetComponentsInChildren<Text>();
		for (int i = 0; i < texts.Length; i++) {
			Destroy(texts[i].gameObject);
		}

		float endSize = 0;

		for (int i = msg.Length - 1; i > -1; i--) {
			RectTransform trans = ((GameObject)Instantiate (prefab)).GetComponent<RectTransform> ();
			trans.SetParent (parent);
			trans.anchoredPosition = -Vector2.up * (endSize + 15);
			trans.GetComponentInChildren<Text> ().text = msg [i].message;

			trans.SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, parent.rect.width);
			Text t = trans.GetComponent<Text> ();
			TextGenerationSettings s = t.GetGenerationSettings (trans.rect.size);
			float slotSize = t.cachedTextGenerator.GetPreferredHeight(msg[i].message, s);
			trans.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, slotSize);
			t.text = msg [i].message;
			t.color = msg [i].color;
			endSize += slotSize;
		}
		parent.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, endSize + 15);
	}
	private void InterfaceMotor () {

		if (MyDebug.haveToUpdate) {
			FillMessages (messagesParent, MyDebug.messages.ToArray ());
			MyDebug.haveToUpdate = false;
		}

		portait.texture = ITextureDrawer.GetFromPerson (character.status);

		bool sp = character.status.canUseRunes;

		spellButton.enabled = character.status.rune > -1 && sp;

		SetTextWithScales (characterInfoText, character.status.GetAllInfo ());

		spellButton.texture = IItemAsset.LoadTexture (character.status.rune);

		for (int i = 0; i < runeStats.Length; i++) {
			string path = "Sprites/UI/Magicka";
			if (i < character.status.spellsToday) {
				path = path + "Active";
			}
			runeStats[i].texture = (Texture)Resources.Load(path);
		}
		for (int i = 0; i < armorStats.Length; i++) {
			string path = "Sprites/UI/Armor";
			if (i < character.status.armorProtection) {
				path = path + "Active";
			}
			armorStats[i].texture = (Texture)Resources.Load(path);
		}

		if (state != IGameMenuState.Runtime) {
			drag = false;
		}

		if (!rui) {
			rui = runtimeUse.image;
		}
		if (!toGameImage) {
			toGameImage = toGame.image;
		}

		runtimeTalk.image.enabled = character.canTalk;

		toGameImage.enabled = (state != IGameMenuState.Runtime && state != IGameMenuState.Dialog);

		rui.enabled = character.canUse;

		int[] items = character.status.items;
		//float delta = Mathf.Abs(slotsParent.rect.height - slotsMask.rect.height);
		//slotsParent.anchoredPosition = Vector2.up * delta * inventoryScroll.value;

		if (!currentItems.Equals(items)) {

			if (lootableChest) {
				PrepareLootItems (lootableChest);
			}
			
			InventorySlot[] buttons = FillItemsTo (slotsParent, items);

			for (int i = 0; i < buttons.Length; i++) {
				int index = i;
				Button bt = buttons [i].button;
				bt.onClick.RemoveAllListeners();
				bt.onClick.AddListener(delegate {

					currentLookableItemIndex = character.status.GetItemByID(buttons[index].id);

				});
			}

			amuletInv.GetComponentInChildren<RawImage> ().texture = IItemAsset.LoadTexture (character.status.amulet);
			armorInv.GetComponentInChildren<RawImage> ().texture = IItemAsset.LoadTexture (character.status.armor);
			weapontInv.GetComponentInChildren<RawImage> ().texture = IItemAsset.LoadTexture (character.status.weapon);
			runeInv.GetComponentInChildren<RawImage> ().texture = IItemAsset.LoadTexture (character.status.rune);

			int ru = character.status.rune;

			ICharacter ch = character;

			amuletInv.onClick.RemoveAllListeners ();
			amuletInv.onClick.AddListener (delegate {
			
				ch.BackToInv(IItemType.Amulet);

			});
				
			armorInv.onClick.RemoveAllListeners ();
			armorInv.onClick.AddListener (delegate {

				ch.BackToInv(IItemType.Armor);

			});

			weapontInv.onClick.RemoveAllListeners ();
			weapontInv.onClick.AddListener (delegate {

				ch.BackToInv(IItemType.Weapon);

			});

			runeInv.onClick.RemoveAllListeners ();
			runeInv.onClick.AddListener (delegate {

				if (ru > -1) {
					ch.BackToInv(IItemAsset.items[ru].type);
				}

			});

			currentItems = items;
		}


		if (probeIndexLookable != currentLookableItemIndex) {
			int index = currentLookableItemIndex;
			use.onClick.RemoveAllListeners();
			use.onClick.AddListener (delegate {

				UseItem(index);

			});

			drop.onClick.RemoveAllListeners();
			drop.onClick.AddListener (delegate {
				
				Drop(index);
				
			});

			probeIndexLookable = currentLookableItemIndex;
		}

		bool cur = currentLookableItemIndex > -1 && currentLookableItemIndex < items.Length;

		inventoryFunctionsLabel.SetActive (cur);



		if (cur) {
			IItem current = IItemAsset.items [items [currentLookableItemIndex]];
			string t = current.Info ();
			SetTextWithScales (itemsInfo, t);
		} else {
			SetTextWithScales (itemsInfo, "-");
		}
	}
	public void Quit () {
		Application.Quit ();
	}
	public void ToMenu () {
		ISpace.LoadMainMenu ();
	}
	public void SaveAndExitButton () {
		state = IGameMenuState.Runtime;
		SetState ();
		IGame.SavePicture (IGame.currentProfile);
		IGame.CaptureGame ();
		IGame.Save (IGame.buffer, IGame.currentProfile);
		Invoke ("ToMenu", 0.5f);
	}
	public void ReloadSaved () {
		ISpace.LoadGameFromIndex (IGame.currentProfile);
	}
	private void CameraMotor () {
		cam.eulerAngles = new Vector3 (45, 45, 0);
		Ray ray = new Ray (character.trans.position + Vector3.up, -cam.forward);
		Vector3 point = ray.GetPoint (100);
		cam.position = Vector3.Slerp(cam.position, point, Time.fixedDeltaTime * 5);
	}
	private void PrepareCamera () {
		cam.eulerAngles = new Vector3 (45, 45, 0);
		Ray ray = new Ray (IGame.buffer.FindByName("Player").position + Vector3.up, -cam.forward);
		Vector3 point = ray.GetPoint (100);
		cam.position = point;
		camMain = cam.GetComponent<Camera> ();
	}
	private void Awake () {
		control = this;
		cam = Camera.main.transform;
	}
	public void UseItem (int index) {
		if (character.status.CanUseItem(character.status.items[index])) {
			if (IItemAsset.items [character.status.items [index]].type == IItemType.EmptyScroll) {
				if (character.status.spellsToday > 0) {
					state = IGameMenuState.RuneCreate;
					character.RemoveItem (index);
					SetState ();
				}
			} else {
				character.UseItem (index);
			}
		}
	}
	public void Drop (int index) {
		character.DropItem (index);
	}
	public void Attack () {
		if (character) {
			character.AttackFromDirection ();
		}
	}
	private IChest lootableChest;
	private void PrepareLootItems (IChest chest) {
		int[] lootItems = chest.data.items;
		int[] mineItems = character.status.items;
		InventorySlot[] lootButtons = FillItemsTo (lootSlotsParent, lootItems);
		InventorySlot[] mineButtons = FillItemsTo (lootMineSlotsParent, mineItems);
		for (int i = 0; i < mineButtons.Length; i++) {
			int index = i;
			mineButtons [i].button.onClick.RemoveAllListeners ();
			mineButtons [i].button.onClick.AddListener (delegate {
				chest.AddFrom(character, character.status.GetItemByID(mineButtons[index].id));
			});
		}
		for (int i = 0; i < lootButtons.Length; i++) {
			int index = i;
			lootButtons [i].button.onClick.RemoveAllListeners ();
			lootButtons [i].button.onClick.AddListener (delegate {
				chest.CastTo(character, chest.GetItemByID(lootButtons[index].id));
			});
		}
		lootableChest = chest;
	}
	public static bool HasInt (int[] array, int element) {
		bool has = false;
		for (int i = 0; i < array.Length; i++) {
			if (array[i] == element) {
				has = true;
				break;
			}
		}
		return has;
	}
	[System.Serializable]
	public struct InventorySlot
	{
		public Button button;
		public int id;

		public InventorySlot(int ID, Button bt)
		{
			id = ID;
			button = bt;
		}
	}
	public InventorySlot[] FillItemsTo (RectTransform parent, int[] items) {
		GameObject prefab = (GameObject)Resources.Load ("Prefabs/Slot");
		Button[] buttons = parent.GetComponentsInChildren<Button>();
		for (int i = 0; i < buttons.Length; i++) {
			Destroy(buttons[i].gameObject);
		}
		List<InventorySlot> bts = new List<InventorySlot> ();

		List<int> has = new List<int> ();
		List<ISlot> upd = new List<ISlot> ();

		for (int i = 0; i < items.Length; i++) {
			if (!HasInt (has.ToArray (), items[i])) {
				upd.Add (new ISlot (items [i]));
				has.Add (items [i]);
			} else {
				int index = -1;
				for (int n = 0; n < upd.Count; n++) {
					if (upd[n].id == items[i]) {
						index = n;
						break;
					}
				}
				if (index > -1) {
					upd [index].Add ();
				}
			}
		}

		float slotSize = prefab.GetComponent<RectTransform>().rect.height;

		for (int i = 0; i < upd.Count; i++) {
			RectTransform trans = ((GameObject)Instantiate (prefab)).GetComponent<RectTransform> ();
			trans.SetParent(parent);
			trans.anchoredPosition = -Vector2.up * slotSize * i;
			trans.GetComponentInChildren<RawImage>().texture = IItemAsset.LoadTexture(upd[i].id);
			string num = "";
			if (upd[i].count > 1) {
				num = "(" + upd [i].count + ")";
			}
			trans.GetComponentInChildren<Text>().text = IItemAsset.items[upd[i].id].name + num;

			trans.SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, parent.rect.width);
			Button bt = trans.GetComponent<Button>();

			Color c = Color.white;

			if (!character.status.CanUseItem(upd[i].id)) {
				c = new Color(1f, 0.5f, 0.5f);
			}

			bt.image.color = c;

			bts.Add (new InventorySlot(upd[i].id, bt));
		}

		parent.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, slotSize * upd.Count);

		return bts.ToArray ();
	}
	private void Start () {
		toInventory.onClick.RemoveAllListeners ();
		toInventory.onClick.AddListener (delegate {

			state = IGameMenuState.Inventory;
			SetState();

		});

		toGame.onClick.RemoveAllListeners ();
		toGame.onClick.AddListener (delegate {
			
			state = IGameMenuState.Runtime;
			SetState();
			
		});

		saveAndToMenu.onClick.RemoveAllListeners ();
		saveAndToMenu.onClick.AddListener (delegate {

			SaveAndExitButton();

		});

		toGameMenu.onClick.RemoveAllListeners ();
		toGameMenu.onClick.AddListener (delegate {

			state = IGameMenuState.Pause;
			SetState();

		});

		toCharacterMenu.onClick.RemoveAllListeners ();
		toCharacterMenu.onClick.AddListener (delegate {

			state = IGameMenuState.CharacterInfo;
			SetState();

		});


		runtimeUse.onClick.RemoveAllListeners ();
		runtimeUse.onClick.AddListener (delegate {

			UseRuntime();

		});

		runtimeAttack.onDrag.RemoveAllListeners ();
		runtimeAttack.onDrag.AddListener (delegate {

			Attack();

		});

		runtimeTalk.onClick.RemoveAllListeners ();
		runtimeTalk.onClick.AddListener (delegate {

			TalkRuntime();

		});

		rune_air.onClick.RemoveAllListeners ();
		rune_air.onClick.AddListener (delegate {

			character.CreateRune(IItemType.ScrollOfAir);
			state = IGameMenuState.Inventory;
			SetState();

		});
		rune_fire.onClick.RemoveAllListeners ();
		rune_fire.onClick.AddListener (delegate {

			character.CreateRune(IItemType.ScrollOfFire);
			state = IGameMenuState.Inventory;
			SetState();

		});
		rune_earth.onClick.RemoveAllListeners ();
		rune_earth.onClick.AddListener (delegate {

			character.CreateRune(IItemType.ScrollOfEarth);
			state = IGameMenuState.Inventory;
			SetState();

		});
		rune_water.onClick.RemoveAllListeners ();
		rune_water.onClick.AddListener (delegate {

			character.CreateRune(IItemType.ScrollOfWater);
			state = IGameMenuState.Inventory;
			SetState();

		});
		rune_god.onClick.RemoveAllListeners ();
		rune_god.onClick.AddListener (delegate {

			character.CreateRune(IItemType.ScrollOfGod);
			state = IGameMenuState.Inventory;
			SetState();

		});

		runeStats = runeStatsParent.GetComponentsInChildren<RawImage> ();
		armorStats = armorStatsParent.GetComponentsInChildren<RawImage> ();

		PrepareCamera ();


		SetState ();
	}
	private void UseRuntime () {
		if (character) {
			IUsable us = character.canUse;
			if (us) {
				character.UseGameObject (character.canUse);
				if (us is IChest) {
					PrepareLootItems ((IChest)us);
					state = IGameMenuState.Loot;
					SetState ();
				}
			}
		}
	}
	private void TalkRuntime () {
		
		if (character.canTalk) {
			state = IGameMenuState.Dialog;
			SetState();
			ToDialog();
		}
	}
	private void JoySinhro () {
		if (probe != drag) {
			joyPanel.gameObject.SetActive(drag);
			joyStick.gameObject.SetActive(drag);

			probe = drag;
		}
	}

	public void OnBeginDrag (PointerEventData data) {
		joyPanel.anchoredPosition = data.position;
		drag = true;
	}
	public static Vector3 FromCameraAxes (Vector3 origin) {
		if (!cam) {
			cam = Camera.main.transform;
		}
		Vector3 forward = cam.forward;
		Vector3 right = cam.right;
		forward = new Vector3 (forward.x, 0, forward.z).normalized;
		right = new Vector3 (right.x, 0, right.z).normalized;
		return forward * origin.z + right * origin.x;
	}
	public void OnDrag (PointerEventData data) {
		Vector2 pos = data.position - joyPanel.anchoredPosition;
		float max = joyPanel.sizeDelta.y / 2;
		if (pos.magnitude > max) {
			pos = pos.normalized * max;
		}
		joyStick.anchoredPosition = pos;
	}
	public void OnEndDrag (PointerEventData data) {
		drag = false;
	}
}