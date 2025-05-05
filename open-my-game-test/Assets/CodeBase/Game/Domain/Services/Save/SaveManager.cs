#nullable enable
using Newtonsoft.Json;
using UnityEngine;

namespace Game.Domain.Services.Save
{
    public class SaveManager
    {
        private const string SaveKey = "SwipeElements_Save";

        public void Save(SaveData data)
        {
            string json = JsonConvert.SerializeObject(data);
            PlayerPrefs.SetString(SaveKey, json);
            PlayerPrefs.Save();
        }

        public SaveData? Load()
        {
            if (!PlayerPrefs.HasKey(SaveKey))
                return null;

            string json = PlayerPrefs.GetString(SaveKey);
            return JsonConvert.DeserializeObject<SaveData>(json);
        }
    }
}