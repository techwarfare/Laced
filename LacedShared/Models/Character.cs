using LacedShared.Classes;
using LacedShared.Enums;
using LacedShared.Libs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LacedShared.Models
{
    public class CharacterParents
    {
        [JsonProperty]
        public int Father { get; protected set; } = 0;
        [JsonProperty]
        public int Mother { get; protected set; } = 0;
        [JsonProperty]
        public float Mix { get; protected set; } = 0.5f;
        public void SetFather(int _father)
        {
            Father = _father;
        }
        public void SetMother(int _mother)
        {
            Mother = _mother;
        }
        public void SetMix(float _mix)
        {
            Mix = _mix;
        }
        public CharacterParents() {}
    }
    public class Overlay
    {
        [JsonProperty]
        public int Index { get; protected set; } = 0;
        [JsonProperty]
        public float Opacity { get; protected set; } = 0f;
        [JsonProperty]
        public bool IsHair { get; protected set; } = false;

        public Overlay() { }

        public Overlay(int _index, float _opacity, bool _ishair)
        {
            Index = _index;
            Opacity = _opacity;
            IsHair = _ishair;
        }

        public void SetIndex(int _index)
        {
            Index = _index;
        }

        public void SetOpacity(float _opacity)
        {
            Opacity = _opacity;
        }
    }
    public class CharacterAppearance
    {
        [JsonProperty]
        public int HairStyle { get; protected set; } = 0;
        [JsonProperty]
        public int HairColor { get; protected set; } = 0;
        [JsonProperty]
        public int HairHighlightColor { get; protected set; } = 0;
        [JsonProperty]
        public Dictionary<int, Overlay> Overlays { get; protected set; } = new Dictionary<int, Overlay>()
        {
            [0] = new Overlay(0, 0f, false),
            [1] = new Overlay(0, 0f, true),
            [2] = new Overlay(0, 0f, true),
            [3] = new Overlay(0, 0f, false),
            [4] = new Overlay(0, 0f, false),
            [5] = new Overlay(0, 0f, false),
            [6] = new Overlay(0, 0f, false),
            [7] = new Overlay(0, 0f, false),
            [8] = new Overlay(0, 0f, false),
            [9] = new Overlay(0, 0f, false),
            [10] = new Overlay(0, 0f, false),
            [11] = new Overlay(0, 0f, false),
            [12] = new Overlay(0, 0f, false)
        };
        public void SetHairStyle(int _style)
        {
            this.HairStyle = _style;
        }
        public void SetHairColor(int _color)
        {
            this.HairColor = _color;
        }
        public void SetHairHighlightColor(int _color)
        {
            this.HairHighlightColor = _color;
        }
        public void SetOverlay(int _key, int _index, float _opacity)
        {
            this.Overlays[_key].SetIndex(_index);
            this.Overlays[_key].SetOpacity(_opacity);
        }
        public CharacterAppearance() { }
    }
    public class Inventory
    {
        public Dictionary<int,InventoryItem> InventoryItems = new Dictionary<int, InventoryItem>();
        public Inventory()
        {

        }
        public Inventory(int _slotAmount)
        {
            for (int i=0;i<_slotAmount;i++)
            {
                InventoryItems.Add(i, new InventoryItem(i));
            }
        }
        public Inventory(Dictionary<int,InventoryItem> _existingInventory)
        {
            InventoryItems = _existingInventory;
        }
    }
    public class Character
    {
        [JsonProperty]
        public int Id { get; protected set; }
        [JsonProperty]
        public int UserId { get; protected set; }
        [JsonProperty]
        public string FirstName { get; protected set; }
        [JsonProperty]
        public string LastName { get; protected set; }
        [JsonProperty]
        public Genders Gender { get; protected set; }
        [JsonProperty]
        public int Wallet { get; protected set; }
        [JsonProperty]
        public int Bank { get; protected set; }
        [JsonProperty]
        public bool IsNew { get; protected set; } = true;
        [JsonProperty]
        public bool IsDead { get; protected set; } = false;
        [JsonProperty]
        public float[] LastPos { get; protected set; }
        [JsonProperty]
        public string Model { get; protected set; }
        [JsonProperty]
        public Garage Garage { get; protected set; }
        [JsonProperty]
        public CharacterParents Parents { get; protected set; } = new CharacterParents();
        [JsonProperty]
        public Dictionary<int, float> FaceFeatures { get; protected set; } = new Dictionary<int, float>()
        {
            [0] = 0f,
            [1] = 0f,
            [2] = 0f,
            [3] = 0f,
            [4] = 0f,
            [5] = 0f,
            [6] = 0f,
            [7] = 0f,
            [8] = 0f,
            [9] = 0f,
            [10] = 0f,
            [11] = 0f,
            [12] = 0f,
            [13] = 0f,
            [14] = 0f,
            [15] = 0f,
            [16] = 0f,
            [17] = 0f,
            [18] = 0f,
            [19] = 0f
        };
        [JsonProperty]
        public CharacterAppearance Appearance { get; protected set; } = new CharacterAppearance();
        [JsonProperty]
        public Dictionary<int, int[]> Clothing { get; protected set; } = new Dictionary<int, int[]>()
        {
            [0] = new int[] { 0, 0 },
            [1] = new int[] { 0, 0 },
            [2] = new int[] { 0, 0 },
            [3] = new int[] { 0, 0 },
            [4] = new int[] { 0, 0 },
            [5] = new int[] { 0, 0 },
            [6] = new int[] { 0, 0 },
            [7] = new int[] { 0, 0 },
            [8] = new int[] { 0, 0 },
            [9] = new int[] { 0, 0 },
            [10] = new int[] { 0, 0 },
            [11] = new int[] { 0, 0 }
        };
        [JsonProperty]
        public Dictionary<int, int[]> Props { get; protected set; } = new Dictionary<int, int[]>()
        {
            [0] = new int[] { 0, 0 },
            [1] = new int[] { 0, 0 },
            [2] = new int[] { 0, 0 },
            [3] = new int[] { 0, 0 },
            [4] = new int[] { 0, 0 },
            [5] = new int[] { 0, 0 },
            [6] = new int[] { 0, 0 },
            [7] = new int[] { 0, 0 },
            [8] = new int[] { 0, 0 },
            [9] = new int[] { 0, 0 }
        };
        [JsonProperty]
        public Inventory Inventory { get; protected set; }
        [JsonProperty]
        public int InventoryWeight { get; protected set; }
        [JsonProperty]
        public int InventorySlots { get; protected set; } = 16;
        [JsonProperty]
        public int PedWeight { get; protected set; }
        [JsonProperty]
        public int Hunger { get; protected set; }
        [JsonProperty]
        public int Thirst { get; protected set; }
        
        public Character() { }
        public Character(int _userID, string _firstName, string _lastName, Genders _gender, int _inventorySpace, int _wallet, int _bank, List<GarageItem> _garageItems, Dictionary<int, InventoryItem> _existingInventory)
        {
            UserId = _userID;
            FirstName = _firstName;
            LastName = _lastName;
            Gender = _gender;
            Wallet = _wallet;
            Bank = _bank;
            if (Gender == Genders.Male)
            {
                this.Model = "mp_m_freemode_01";
                this.Clothing[0] = new int[] { 0, 0 }; // FACE
                this.Clothing[1] = new int[] { 0, 0 }; // HEAD
                this.Clothing[2] = new int[] { 0, 0 }; // HAIR
                this.Clothing[3] = new int[] { 0, 0 }; // TORSO
                this.Clothing[4] = new int[] { 5, 7 }; // LEGS
                this.Clothing[5] = new int[] { 0, 0 }; // HANDS
                this.Clothing[6] = new int[] { 6, 0 }; // SHOES
                this.Clothing[7] = new int[] { 0, 0 }; // SPECIAL1
                this.Clothing[8] = new int[] { 57, 0 }; // SPECIAL2
                this.Clothing[9] = new int[] { 0, 0 }; // SPECIAL3
                this.Clothing[10] = new int[] { 0, 0 }; // TEXTURES
                this.Clothing[11] = new int[] { 146, 0 }; // TORSO2
            }
            else
            {
                this.Model = "mp_f_freemode_01";
                this.Clothing[0] = new int[] { 0, 0 }; // FACE
                this.Clothing[1] = new int[] { 0, 0 }; // HEAD
                this.Clothing[2] = new int[] { 0, 0 }; // HAIR
                this.Clothing[3] = new int[] { 4, 0 }; // TORSO
                this.Clothing[4] = new int[] { 66, 6 }; // LEGS
                this.Clothing[5] = new int[] { 0, 0 }; // HANDS
                this.Clothing[6] = new int[] { 5, 0 }; // SHOES
                this.Clothing[7] = new int[] { 0, 0 }; //SPECIAL1
                this.Clothing[8] = new int[] { 2, 0 }; // SPECIAL2
                this.Clothing[9] = new int[] { 0, 0 }; // SPECIAL3
                this.Clothing[10] = new int[] { 0, 0 }; // TEXTURES
                this.Clothing[11] = new int[] { 118, 0 }; // TORSO2
            }

            Garage = new Garage(this.Id, _garageItems);
            Inventory = new Inventory(_existingInventory) ?? new Inventory(InventorySlots);
        }
        public void SetNoLongerNew()
        {
            IsNew = false;
        }
        public void SetDeadStatus(bool _isDead)
        {
            IsDead = _isDead;
        }
        public void SetLastPosition(float[] _position)
        {
            LastPos[0] = _position[0];
            LastPos[1] = _position[1];
            LastPos[2] = _position[2];
        }
        public void SetParents(int _father, int _mother, float _mix)
        {
            Parents.SetFather(_father);
            Parents.SetMother(_mother);
            Parents.SetMix(_mix);
        }
        public void SetFaceFeatures(Dictionary<int, float> _features)
        {
            FaceFeatures = _features;
        }
        public void SetFaceFeature(int _key, float _featureMix)
        {
            FaceFeatures[_key] = _featureMix;
        }
        public void SetProps(int _key, int[] args)
        {
            Props[_key] = new int[] { args[0], args[1] };
        }
        public void IncreaseThirst(int _amount)
        {
            this.Thirst = Thirst + _amount;
        }
        public void IncreaseHunger(int _amount)
        {
            this.Hunger = Hunger + _amount;
        }
        public bool BuyItem(int _amount)
        {
            if (Wallet >= _amount)
            {
                Wallet = Wallet - _amount;
                return true;
            }
            else if (Bank >= _amount)
            {
                Bank = Bank - _amount;
                return true;
            }
            return false;
        }
        public void SellItem(int _amount)
        {
            Wallet = Wallet + _amount;
        }
        public bool SellVehicle(GarageItem _garageItem)
        {
            try
            {
                Garage.garageItems.Remove(_garageItem);
                return true;
            }
            catch (Exception _ex)
            {
                Utils.Throw(_ex);
                return false;
            }
        }
    }
}
