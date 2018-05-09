using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

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
	public static Transform camParent;
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
	public GameObject skillMenuObj;

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

	public Text answer;

	public Button runtimeUse;
	Text rut;
	Image rui;
	public Button runtimeTalk;
	public IDragButton runtimeAttack;

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
	public RectTransform dialogButtonsParent;
	public RectTransform skillButtonsParent;

	public SkillButton[] skillButtons;
	public SkillButton[] selectSkillButtons;
	public Text skillInfo;
	private int currentToSetSkill = 0;

	public Text characterInfoText;

	public float deathTime = 0;
	public Vector2 cameraEuler;

	public Text messagesText;

	public enum GameMenuState
	{
		Runtime,
		Pause,
		Inventory,
		Dialog,
		Death,
		Screenshot,
		RuneCreate,
		CharacterInfo,
		Loot,
		Skills
	}

	public GameMenuState state
	{
		get {
			return st;
		}
		set {
			st = value;
			SetState ();
			IFontSetter.SetFontForall ();
		}
	}
	private GameMenuState st;

	private bool stateChanged = false;

	public void SetState () {
		runtimeObj.SetActive (state == GameMenuState.Runtime);
		inventoryObj.SetActive (state == GameMenuState.Inventory);
		pauseObj.SetActive (state == GameMenuState.Pause);
		dialogObj.SetActive (state == GameMenuState.Dialog);
		deathMenuObj.SetActive (state == GameMenuState.Death);
		runeMenuObj.SetActive (state == GameMenuState.RuneCreate);
		characterMenuObj.SetActive (state == GameMenuState.CharacterInfo);
		lootMenuObj.SetActive (state == GameMenuState.Loot);
		skillMenuObj.SetActive (state == GameMenuState.Skills);

		if (state != GameMenuState.Loot && lootableChest) {
			lootableChest = null;
		}
		if (state == GameMenuState.CharacterInfo) {
			portait.texture = ITextureDrawer.GetFromPerson (character.status);
		}

		if (state == GameMenuState.Skills) {
			InventorySlot[] slots = FillItemsTo<Skill> (skillButtonsParent, character.status.skills.forMyClass);
			foreach (var item in slots) {
				item.button.onClick.RemoveAllListeners ();
				item.button.onClick.AddListener (delegate {
					SetToQuickSkill(item.skill);
				});
			}
			for (int i = 0; i < 3; i++) {
				currentToSetSkill = i;
				SetToQuickSkill (SkillSystem.GetByName(character.status.skills.quickSkills [i]));
			}
			currentToSetSkill = 0;
		}

		stateChanged = true;

		toGame.gameObject.SetActive (state != GameMenuState.RuneCreate);

		if (state != GameMenuState.Runtime) {
			Time.timeScale = 0;
		} else {
			Time.timeScale = 1;
		}

		IFontSetter.SetFontForall ();
	}

	public void SetToQuickSkill (Skill skill) {
		character.status.skills.quickSkills[currentToSetSkill] = skill.name;
		SetTextWithScales (skillInfo, skill.Info ());
		selectSkillButtons[currentToSetSkill].SetWithSkill(skill);
	}

	public void OutDialog () {
		state = GameMenuState.Runtime;
	}

	public void ToDialog () {
		ICharacter with = character.canTalk;
		dialog_window.with = with;
		dialog_window.for_answer = answer;
		dialog_window.parent = dialogButtonsParent;
		dialog_window.control = this;
		dialogPortait.texture = ITextureDrawer.GetFromPerson (with.status);
		dialog_window.InitializeNode ();

		//dialog_window.PrepareStage ();
	}
	public IDialogWindow dialog_window;


	public void ToSkillsMenu () {
		state = GameMenuState.Skills;
	}

	private bool drag = false;
	bool probe = true;
	public bool characterDead = false;

	private void Update () {

		TimeClass.time += Time.deltaTime * TimeClass.timeSpeed;

		if (Time.time - deathTime > 5 && characterDead) {
			if (state != GameMenuState.Death) {
				state = GameMenuState.Death;
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
		// fixed
	}
	public static void SetTextWithScales (Text t, string msg) {
		IFontSetter.SetFont (t);
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
	private void InterfaceMinUpdate () {

		SetTextWithScales (characterInfoText, character.status.GetAllInfo ());

		Skill[] skills = SkillSystem.GetFromNames(character.status.skills.quickSkills);

		foreach (var item in skillButtons) {
			item.onClick = delegate() {
				return;
			};
			item.img.enabled = false;
		}

		for (int i = 0; i < skills.Length; i++) {
			skillButtons [i].img.texture = skills [i].image;
			Skill sk = skills [i];
			skillButtons [i].onClick = () => character.CommitActionNow (sk);
			skillButtons [i].img.enabled = SkillSystem.HasInDatabase (skills [i].name);
		}

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

		int[] items = character.status.items;

		bool cur = currentLookableItemIndex > -1 && currentLookableItemIndex < items.Length;

		inventoryFunctionsLabel.SetActive (cur);

		if (cur) {
			Item current = ItemsAsset.items [items [currentLookableItemIndex]];
			string t = current.Info ();
			SetTextWithScales (itemsInfo, t);
		} else {
			SetTextWithScales (itemsInfo, "-");
		}
	}
	private void CheckUse (IUsable can) {
		if (!rui || !rut) {
			rui = runtimeUse.image;
			rut = runtimeUse.GetComponentInChildren<Text> ();
		}
		if (rui) {
			rui.enabled = can;
		}
		if (rut) {
			rut.enabled = can;
			if (can) {
				rut.text = IUsable.GetUseText (can);
			}
		}
	}
	private void InterfaceMotor () {

		bool haveToMinUpdate = false;

		if (MyDebug.haveToUpdate && MyDebug.messages.Count > 0) {
			SetTextWithScales (messagesText, MyDebug.messages[MyDebug.messages.Count - 1].message);
			MyDebug.haveToUpdate = false;
			messagesText.color = MyDebug.messages [MyDebug.messages.Count - 1].color;
		}

		if (state != GameMenuState.Runtime) {
			drag = false;
		}
		if (!toGameImage) {
			toGameImage = toGame.image;
		}

		runtimeTalk.image.enabled = character.canTalk;

		toGameImage.enabled = (state != GameMenuState.Runtime && state != GameMenuState.Dialog);

		IUsable can = character.canUse;

		CheckUse (can);

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

			amuletInv.GetComponentInChildren<RawImage> ().texture = ItemsAsset.LoadTexture (character.status.amulet);
			armorInv.GetComponentInChildren<RawImage> ().texture = ItemsAsset.LoadTexture (character.status.armor);
			weapontInv.GetComponentInChildren<RawImage> ().texture = ItemsAsset.LoadTexture (character.status.weapon);
			runeInv.GetComponentInChildren<RawImage> ().texture = ItemsAsset.LoadTexture (character.status.rune);

			int ru = character.status.rune;

			ICharacter ch = character;

			amuletInv.onClick.RemoveAllListeners ();
			amuletInv.onClick.AddListener (delegate {
			
				ch.BackToInv(ItemType.Amulet);

			});
				
			armorInv.onClick.RemoveAllListeners ();
			armorInv.onClick.AddListener (delegate {

				ch.BackToInv(ItemType.Armor);

			});

			weapontInv.onClick.RemoveAllListeners ();
			weapontInv.onClick.AddListener (delegate {

				ch.BackToInv(ItemType.Weapon);

			});

			runeInv.onClick.RemoveAllListeners ();
			runeInv.onClick.AddListener (delegate {

				if (ru > -1) {
					ch.BackToInv(ItemsAsset.items[ru].type);
				}

			});

			currentItems = items;
			haveToMinUpdate = true;
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

			haveToMinUpdate = true;
		}

		if (stateChanged) {
			haveToMinUpdate = true;
			stateChanged = false;
		}


		if (haveToMinUpdate) {
			InterfaceMinUpdate ();
		}
	}
	public void Quit () {
		Application.Quit ();
	}
	public void ToMenu () {
		ISpace.LoadMainMenu ();
	}
	public void SaveAndExitButton () {
		state = GameMenuState.Runtime;
		SGame.SavePicture (SGame.currentProfile);
		SGame.CaptureGame ();
		SGame.Save (SGame.buffer, SGame.currentProfile);
		Invoke ("ToMenu", 0.5f);
	}
	public void ReloadSaved () {
		ISpace.LoadGameFromIndex (SGame.currentProfile);
	}
	public static Vector3 headHeight
	{
		get
		{
			return Vector3.up * 1.6f;
		}
	}
	public static Vector3 cameraHead {
		get {
			Vector3 h = Vector3.zero;
			if (control && character) {
				h = character.agent.nextPosition + headHeight;
			}
			return h;
		}
	}
	public void CameraMotor () {
		if (character && character.agent) {
			Vector2 v = ICameraDragPanel.dragVector;

			cameraEuler += new Vector2 (-v.y, v.x) * Time.fixedDeltaTime * 15;

			ICameraDragPanel.dragVector = Vector2.zero;

			Vector3 campos = cameraHead;

			cameraEuler.x = Mathf.Clamp (cameraEuler.x, -89, 89);
			camParent.rotation = Quaternion.Slerp(camParent.rotation, Quaternion.Euler((Vector3)cameraEuler), Time.fixedDeltaTime * 7);

			camParent.position = Vector3.Slerp (camParent.position, campos, Time.fixedDeltaTime * 7);
			Ray ray = new Ray (camParent.position, -camParent.forward);

			Vector3 dop = Vector3.zero;
			float dist = 3;
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, dist)) {
				dist = hit.distance;
				dop = hit.normal * 0.2f;
			}

			dop = cam.InverseTransformDirection (dop);

			cam.localPosition = Vector3.Slerp(cam.localPosition, -Vector3.forward * dist + dop, Time.fixedDeltaTime * 7);
		}
	}
	private void PrepareCamera () {
		camParent.eulerAngles = SGame.buffer.cameraEuler;
		camParent.position = (Vector3)SGame.buffer.FindByName("Player").position + Vector3.up * 1.6f;
		cam.localPosition = -Vector3.forward * 3;
		camMain = cam.GetComponent<Camera> ();
	}
	private void Awake () {
		control = this;
		cam = Camera.main.transform;
		camParent = cam.parent;
	}
	public void UseItem (int index) {
		if (character.status.CanUseItem(character.status.items[index])) {
			if (ItemsAsset.items [character.status.items [index]].type == ItemType.EmptyScroll) {
				if (character.status.spellsToday > 0) {
					state = GameMenuState.RuneCreate;
					character.RemoveItem (index);
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
			character.AttackFromDirection (SVector.FlatAndNormallize (cam.forward));
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
		public enum SlotType
		{
			Item,
			Skill
		}
		public Button button;
		public int id
		{
			get {
				return (int)identificator;
			}
		}
		public Skill skill
		{
			get {
				return (Skill)identificator;
			}
		}
		private object identificator;

		public InventorySlot(int ID, Button bt)
		{
			identificator = ID;
			button = bt;
		}
		public InventorySlot(Skill ID, Button bt)
		{
			identificator = ID;
			button = bt;
		}
	}
	public InventorySlot[] FillItemsTo (RectTransform parent, int[] items) {
		return FillItemsTo <int> (parent, items);
	}
	public InventorySlot[] FillItemsTo <T> (RectTransform parent, IEnumerable<T> items) {

		InventorySlot.SlotType sType = InventorySlot.SlotType.Item;

		if (typeof(T) == typeof(Skill)) {
			sType = InventorySlot.SlotType.Skill;
		}

		GameObject prefab = (GameObject)Resources.Load ("Prefabs/Slot");
		Button[] buttons = parent.GetComponentsInChildren<Button>();
		for (int i = 0; i < buttons.Length; i++) {
			Destroy(buttons[i].gameObject);
		}
		List<InventorySlot> bts = new List<InventorySlot> ();

		List<Slot> upd = new List<Slot> ();

		if (sType == InventorySlot.SlotType.Item) {
			IEnumerable<int> its = items.Cast<int> ();
			List<int> has = new List<int> ();
			foreach (var item in its) {
				if (!HasInt (has.ToArray (), item)) {
					upd.Add (new Slot (item));
					has.Add (item);
				} else {
					int index = -1;
					for (int n = 0; n < upd.Count; n++) {
						if (upd [n].id == item) {
							index = n;
							break;
						}
					}
					if (index > -1) {
						upd [index].Add ();
					}
				}
			}
		} else {
			IEnumerable<Skill> its = items.Cast<Skill> ();
			foreach (var item in its) {
				upd.Add (new Slot (item));
			}
		}

		float slotSize = prefab.GetComponent<RectTransform>().rect.height;

		parent.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, slotSize * upd.Count);

		for (int i = 0; i < upd.Count; i++) {
			RectTransform trans = ((GameObject)Instantiate (prefab)).GetComponent<RectTransform> ();
			trans.SetParent(parent);
			trans.anchoredPosition = -Vector2.up * slotSize * i;
			RawImage r = trans.GetComponentInChildren<RawImage> ();
			switch (sType) {
			case InventorySlot.SlotType.Item:
				r.texture = ItemsAsset.LoadTexture(upd[i].id);
				break;
			case InventorySlot.SlotType.Skill:
				r.texture = upd [i].skill.image;
				break;
			}
			string num = "";
			if (upd[i].count > 1) {
				num = "(" + upd [i].count + ")";
			}
			Text t = trans.GetComponentInChildren<Text> ();
			IFontSetter.SetFont (t);

			if (sType == InventorySlot.SlotType.Item) {
				t.text = ItemsAsset.items [upd [i].id].name + num;
			} else {
				t.text = upd [i].skill.LitraName ();
			}
			float w = parent.rect.width;
			w = w > 0 ? w : parent.parent.GetComponent<RectTransform> ().rect.width;
			w = w > 0 ? w : parent.parent.parent.GetComponent<RectTransform> ().rect.width;
			trans.SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, w);
			Button bt = trans.GetComponent<Button>();

			Color c = Color.white;

			if (sType == InventorySlot.SlotType.Item) {
				if (!character.status.CanUseItem (upd [i].id)) {
					c = new Color (1f, 0.5f, 0.5f);
				}
			}

			bt.image.color = c;

			bts.Add (sType == InventorySlot.SlotType.Item ? new InventorySlot (upd[i].id, bt) : new InventorySlot (upd[i].skill, bt));
		}

		return bts.ToArray ();
	}
	private void EffectsUpdate () {
		SkillEffect.EverySecUpdate ();
		Invoke ("EffectsUpdate", 1);
	}
	private void Start () {
		toInventory.onClick.RemoveAllListeners ();
		toInventory.onClick.AddListener (delegate {

			state = GameMenuState.Inventory;

		});

		toGame.onClick.RemoveAllListeners ();
		toGame.onClick.AddListener (delegate {
			
			state = GameMenuState.Runtime;
			
		});

		saveAndToMenu.onClick.RemoveAllListeners ();
		saveAndToMenu.onClick.AddListener (delegate {

			SaveAndExitButton();

		});

		toGameMenu.onClick.RemoveAllListeners ();
		toGameMenu.onClick.AddListener (delegate {

			state = GameMenuState.Pause;

		});

		toCharacterMenu.onClick.RemoveAllListeners ();
		toCharacterMenu.onClick.AddListener (delegate {

			state = GameMenuState.CharacterInfo;

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

			character.CreateRune(ItemType.ScrollOfAir);
			state = GameMenuState.Inventory;

		});
		rune_fire.onClick.RemoveAllListeners ();
		rune_fire.onClick.AddListener (delegate {

			character.CreateRune(ItemType.ScrollOfFire);
			state = GameMenuState.Inventory;

		});
		rune_earth.onClick.RemoveAllListeners ();
		rune_earth.onClick.AddListener (delegate {

			character.CreateRune(ItemType.ScrollOfEarth);
			state = GameMenuState.Inventory;

		});
		rune_water.onClick.RemoveAllListeners ();
		rune_water.onClick.AddListener (delegate {

			character.CreateRune(ItemType.ScrollOfWater);
			state = GameMenuState.Inventory;

		});
		rune_god.onClick.RemoveAllListeners ();
		rune_god.onClick.AddListener (delegate {

			character.CreateRune(ItemType.ScrollOfGod);
			state = GameMenuState.Inventory;

		});

		for (int i = 0; i < selectSkillButtons.Length; i++) {
			int index = i;
			selectSkillButtons [i].onClick = delegate() {
				currentToSetSkill = index;
				string sk = character.status.skills.quickSkills[currentToSetSkill];
				if (SkillSystem.HasInDatabase(sk)) {
					SetToQuickSkill(new Skill());
				}
			};
		}

		runeStats = runeStatsParent.GetComponentsInChildren<RawImage> ();
		armorStats = armorStatsParent.GetComponentsInChildren<RawImage> ();

		PrepareCamera ();

		state = GameMenuState.Runtime;

		Invoke ("EffectsUpdate", 1);

		CheckUse (null);
	}
	private void UseRuntime () {
		if (character) {
			IUsable us = character.canUse;
			if (us) {
				character.UseGameObject (character.canUse);
				if (us is IChest) {
					PrepareLootItems ((IChest)us);
					state = GameMenuState.Loot;
				}
			}
		}
	}
	private void TalkRuntime () {
		
		if (character.canTalk) {
			state = GameMenuState.Dialog;
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