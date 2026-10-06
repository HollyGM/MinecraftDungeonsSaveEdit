using MCDSaveEdit.Data;
using MCDSaveEdit.Save.Models.Profiles;
using MCDSaveEdit.Services;
using System.Collections.Generic;
using System.Linq;
#nullable enable

namespace MCDSaveEdit.ViewModels
{
    public class ProfileViewModel
    {
        public static readonly Dictionary<string, string> supportedFileTypesDict = new Dictionary<string, string>
        {
            {"*" + Constants.ENCRYPTED_FILE_EXTENSION, R.ENCRYPTED_CHARACTER_SAVE_FILES },
            {"*" + Constants.DECRYPTED_FILE_EXTENSION, R.DECRYPTED_CHARACTER_SAVE_FILES },
            {"*.*", R.ALL_FILES },
        };

        public MainEquipmentViewModel mainEquipmentModel;
        public StorageChestEquipmentViewModel storageChestEquipmentModel;

        public string? filePath { get; set; }

        private Property<ProfileSaveFile?> _profile = new Property<ProfileSaveFile?>(null);
        public IReadWriteProperty<ProfileSaveFile?> profile { get { return _profile; } }

        public IReadWriteProperty<bool?> unlockPortal;
        public IReadWriteProperty<ulong?> emeralds;
        public IReadWriteProperty<ulong?> gold;
        public IReadWriteProperty<ulong?> eyeOfEnder;

        public ProfileViewModel()
        {
            mainEquipmentModel = new MainEquipmentViewModel(_profile);
            storageChestEquipmentModel = new StorageChestEquipmentViewModel(_profile);

            emeralds = mapCurrency(Constants.EMERALD_CURRENCY_NAME);
            gold = mapCurrency(Constants.GOLD_CURRENCY_NAME);
            eyeOfEnder = mapCurrency(Constants.EYE_OF_ENDER_CURRENCY_NAME);

            unlockPortal = _profile.map(p =>
                p?.StrongholdProgess?.Where(x => x.Key.EndsWith("Unlocked")).All(x => x.Value),
                (p, value) => {
                    if (p == null || value == null) { return; }
                    
                    p.StrongholdProgess = p.StrongholdProgess?
                        .ToDictionary(x => x.Key, x => x.Key.EndsWith("Unlocked") ? value.Value : x.Value);
                });

        }

        private IReadWriteProperty<ulong?> mapCurrency(string currencyType)
        {
            return _profile.map<ProfileSaveFile?, ulong?>(
                p => p?.Currency?.FirstOrDefault(c => c.Type == currencyType)?.Count,
                (p, value) => {
                    if (p == null || value == null) { return; }

                    var currencies = p.Currency ?? new Currency[0];
                    Currency currency =
                        currencies.FirstOrDefault(c => c.Type == currencyType) ??
                        new Currency { Type = currencyType };
                    currency.Count = value.Value;
                    p.Currency = (new[] { currency })
                        .Concat(currencies.Where(c => c.Type != currencyType))
                        .OrderBy(c => c.Type).ToArray();
                });
        }

    }
}
