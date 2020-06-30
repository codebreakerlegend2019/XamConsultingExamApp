using MonkeyCache.SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XamConsultingTest.Interfaces;
using XamConsultingTest.Models;

namespace XamConsultingTest.Services
{
    public class EquipmentService : IGetAll<Equipment>, IGet<Equipment> ,IPost<Equipment>,IPut<Equipment>
    {
        private const string _equipmentBarrelKey = "Equipments";
        public List<Equipment> GetAll()
        {
            var cacheEquipments = Barrel.Current.Get<List<Equipment>>(_equipmentBarrelKey);
            if (cacheEquipments == null)
                return new List<Equipment>(0);
            return cacheEquipments;
        }

        private int generateId()
        {
            var lastItem = GetAll().OrderByDescending(x => x.Id).FirstOrDefault();
            if (lastItem == null)
                return 1;
            return lastItem.Id+1;
        }

        public void Post(Equipment equipment)
        {
            var cacheEquipments = GetAll();
            equipment.Id = generateId();
            cacheEquipments.Add(equipment);
            Barrel.Current.Add(_equipmentBarrelKey, cacheEquipments, new TimeSpan(365, 24, 60, 60, 60));
        }

        public void Put(Equipment equipment)
        {
            var cacheEquipments = GetAll();
            foreach(var cacheEquipment in cacheEquipments)
            {
                if(cacheEquipment.Id == equipment.Id)
                {
                    if (cacheEquipment.Name != equipment.Name)
                        cacheEquipment.Name = equipment.Name;
                    if (cacheEquipment.Comments != equipment.Comments)
                        cacheEquipment.Comments = equipment.Comments;
                    if (cacheEquipment.PhotoUri != equipment.PhotoUri)
                        cacheEquipment.PhotoUri = equipment.PhotoUri;
                    break;
                }
            }
            Barrel.Current.Empty(_equipmentBarrelKey);
            Barrel.Current.Add(_equipmentBarrelKey, cacheEquipments, new TimeSpan(365, 24, 60, 60, 60));

        }

        public Equipment Get(int id)
        {
            return GetAll().FirstOrDefault(x => x.Id ==id);
        }
    }
}
