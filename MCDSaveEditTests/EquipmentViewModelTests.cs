using MCDSaveEdit;
using MCDSaveEdit.Data;
using MCDSaveEdit.Save.Models.Enums;
using MCDSaveEdit.Save.Models.Profiles;
using MCDSaveEdit.Services;
using MCDSaveEdit.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
#nullable enable

namespace MCDSaveEditTests.ViewModelTests
{
    [TestClass]
    public class MainEquipmentViewModelTests
    {
        [TestMethod]
        public void TestConstructor()
        {
            var property = new Property<ProfileSaveFile?>(null);
            var instance = new MainEquipmentViewModel(property);
            Assert.IsNotNull(instance);
        }

        [TestMethod]
        public void TestDefaultValues()
        {
            var property = new Property<ProfileSaveFile?>(new ProfileSaveFile());
            var instance = new MainEquipmentViewModel(property);
            Assert.AreEqual(true, instance.equipmentCanExist.value);
            Assert.AreEqual(Constants.MINIMUM_CHARACTER_LEVEL, instance.level.value);
            Assert.AreEqual(ItemFilterEnum.All, instance.filter.value);
            Assert.AreEqual(0, instance.filteredItemList.value.Count());
            Assert.AreEqual(0, instance.equippedItemList.value.Count());
            Assert.AreEqual(0, instance.characterPower.value);
        }

        [TestMethod]
        public void TestAddEquippedItems()
        {
            var property = new Property<ProfileSaveFile?>(new ProfileSaveFile());
            var instance = new MainEquipmentViewModel(property);
            var melee = Constants.createDefaultItemForEquipmentSlot(EquipmentSlotEnum.MeleeGear);
            instance.addEquippedItem(melee);
            var armor = Constants.createDefaultItemForEquipmentSlot(EquipmentSlotEnum.ArmorGear);
            instance.addEquippedItem(armor);
            var ranged = Constants.createDefaultItemForEquipmentSlot(EquipmentSlotEnum.RangedGear);
            instance.addEquippedItem(ranged);
            Assert.AreEqual(0, instance.filteredItemList.value.Count());
            Assert.AreEqual(3, instance.equippedItemList.value.Count());
            Assert.AreEqual(1, instance.characterPower.value);
        }

        [TestMethod]
        public void TestAddEquippedItemsMultipleTimes()
        {
            var property = new Property<ProfileSaveFile?>(new ProfileSaveFile());
            var instance = new MainEquipmentViewModel(property);
            var melee = Constants.createDefaultItemForEquipmentSlot(EquipmentSlotEnum.MeleeGear);
            var armor = Constants.createDefaultItemForEquipmentSlot(EquipmentSlotEnum.ArmorGear);
            var ranged = Constants.createDefaultItemForEquipmentSlot(EquipmentSlotEnum.RangedGear);
            instance.addEquippedItem(melee);
            instance.addEquippedItem(armor);
            instance.addEquippedItem(ranged);

            var melee2 = Constants.createDefaultItemForEquipmentSlot(EquipmentSlotEnum.MeleeGear);
            melee2.Type = "AnotherSword";
            instance.addEquippedItem(melee2);
            Assert.AreEqual(0, instance.filteredItemList.value.Count());
            Assert.AreEqual(3, instance.equippedItemList.value.Count());
            Assert.AreEqual(1, instance.characterPower.value);
        }

        [TestMethod]
        public void TestAddItemsToList()
        {
            var property = new Property<ProfileSaveFile?>(new ProfileSaveFile());
            var instance = new MainEquipmentViewModel(property);
            var melee = Constants.createDefaultItemForFilter(ItemFilterEnum.MeleeWeapons);
            instance.addItemToList(melee);
            var armor = Constants.createDefaultItemForFilter(ItemFilterEnum.Armor);
            instance.addItemToList(armor);
            var ranged = Constants.createDefaultItemForFilter(ItemFilterEnum.RangedWeapons);
            instance.addItemToList(ranged);
            Assert.AreEqual(3, instance.filteredItemList.value.Count());
            Assert.AreEqual(0, instance.equippedItemList.value.Count());
            Assert.AreEqual(0, instance.characterPower.value);
        }

        [TestMethod]
        public void TestMoveEquippedItemToStorageChest()
        {
            var property = new Property<ProfileSaveFile?>(new ProfileSaveFile());
            var inventory = new MainEquipmentViewModel(property);
            var chest = new StorageChestEquipmentViewModel(property);
            var melee = Constants.createDefaultItemForEquipmentSlot(EquipmentSlotEnum.MeleeGear);
            inventory.addEquippedItem(melee);

            inventory.removeItem(melee);
            chest.addItemToList(melee);

            Assert.IsNull(melee.EquipmentSlot);
            Assert.AreEqual(0, melee.InventoryIndex);
            Assert.AreEqual(0, inventory.equippedItemList.value.Count());
            Assert.AreEqual(1, chest.filteredItemList.value.Count());
        }

        [TestMethod]
        public void TestItemsWithoutInventoryIndexAreListed()
        {
            var profile = new ProfileSaveFile() {
                Items = new[] {
                    new Item() { Type = "Sword", InventoryIndex = 1 },
                    new Item() { Type = "Bow" },
                },
            };
            var instance = new MainEquipmentViewModel(new Property<ProfileSaveFile?>(profile));
            var items = instance.filteredItemList.value.ToArray();
            Assert.AreEqual(2, items.Length);
            Assert.AreEqual("Bow", items[1].Type);
        }

        [TestMethod]
        public void TestAddEnchantmentSlotCompletesIncompleteSet()
        {
            var item = new Item() {
                Type = "Sword",
                InventoryIndex = 0,
                Enchantments = Enumerable.Range(0, 4).Select(_ => new Enchantment() { Id = Constants.DEFAULT_ENCHANTMENT_ID }).ToArray(),
            };
            var profile = new ProfileSaveFile() { Items = new[] { item } };
            var instance = new MainEquipmentViewModel(new Property<ProfileSaveFile?>(profile));
            instance.selectItem(item);

            instance.addEnchantmentSlot(instance);
            Assert.AreEqual(6, item.Enchantments!.Length);

            instance.addEnchantmentSlot(instance);
            Assert.AreEqual(9, item.Enchantments!.Length);

            instance.addEnchantmentSlot(instance);
            Assert.AreEqual(Constants.MAXIMUM_ENCHANTMENT_OPTIONS_PER_ITEM, item.Enchantments!.Length);
        }

        [TestMethod]
        public void TestSaveEnchantmentThatIsNotInTheList()
        {
            var enchantment = new Enchantment() { Id = "Sharpness", Level = 1 };
            var netheriteEnchantment = new Enchantment() { Id = "Smiting", Level = 1 };
            var item = new Item() {
                Type = "Sword",
                InventoryIndex = 0,
                Enchantments = new[] { enchantment },
                NetheriteEnchant = netheriteEnchantment,
            };
            var profile = new ProfileSaveFile() { Items = new[] { item } };
            var instance = new MainEquipmentViewModel(new Property<ProfileSaveFile?>(profile));
            instance.selectItem(item);
            instance.selectEnchantment(netheriteEnchantment);

            instance.saveEnchantment(netheriteEnchantment);

            CollectionAssert.AreEqual(new[] { enchantment }, item.Enchantments);
        }

        [TestMethod]
        public void TestHighLevelSetsPositiveExperience()
        {
            var profile = new ProfileSaveFile();
            var instance = new MainEquipmentViewModel(new Property<ProfileSaveFile?>(profile));
            instance.level.setValue = 3_000;
            Assert.IsTrue(profile.Xp > 0);
            Assert.AreEqual(3_000, instance.level.value);
        }

    }

    [TestClass]
    public class StorageChestEquipmentViewModelTests
    {
        [TestMethod]
        public void TestConstructor()
        {
            var property = new Property<ProfileSaveFile?>(null);
            var instance = new StorageChestEquipmentViewModel(property);
            Assert.IsNotNull(instance);
        }

        [TestMethod]
        public void TestDefaultValues()
        {
            var property = new Property<ProfileSaveFile?>(new ProfileSaveFile());
            var instance = new StorageChestEquipmentViewModel(property);
            Assert.AreEqual(ItemFilterEnum.All, instance.filter.value);
            Assert.AreEqual(true, instance.equipmentCanExist.value);
            Assert.AreEqual(Constants.MINIMUM_CHARACTER_LEVEL, instance.level.value);
            Assert.AreEqual(0, instance.remainingEnchantmentPoints.value);
            Assert.AreEqual(0, instance.filteredItemList.value.Count());
            Assert.AreEqual(0, instance.equippedItemList.value.Count());
            Assert.AreEqual(0, instance.characterPower.value);
        }

        [TestMethod]
        public void TestAddItemsToList()
        {
            var property = new Property<ProfileSaveFile?>(new ProfileSaveFile());
            var instance = new StorageChestEquipmentViewModel(property);
            var melee = Constants.createDefaultItemForFilter(ItemFilterEnum.MeleeWeapons);
            instance.addItemToList(melee);
            var armor = Constants.createDefaultItemForFilter(ItemFilterEnum.Armor);
            instance.addItemToList(armor);
            var ranged = Constants.createDefaultItemForFilter(ItemFilterEnum.RangedWeapons);
            instance.addItemToList(ranged);
            Assert.AreEqual(3, instance.filteredItemList.value.Count());
            Assert.AreEqual(0, instance.equippedItemList.value.Count());
            Assert.AreEqual(0, instance.characterPower.value);
        }


    }

    [TestClass]
    public class ProfileViewModelTests
    {
        [TestMethod]
        public void TestCurrencyWithoutCurrencyArray()
        {
            var instance = new ProfileViewModel();
            instance.profile.setValue = new ProfileSaveFile();
            Assert.IsNull(instance.emeralds.value);

            instance.emeralds.setValue = 5;
            Assert.AreEqual(5UL, instance.emeralds.value);
            Assert.AreEqual(1, instance.profile.value!.Currency.Length);
        }
    }
}
