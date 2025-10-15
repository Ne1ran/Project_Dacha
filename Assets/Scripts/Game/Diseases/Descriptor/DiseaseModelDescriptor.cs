using System;
using System.Collections.Generic;
using Game.Diseases.Model;
using Game.Plants.Model;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Diseases.Descriptor
{
    [Serializable]
    public class DiseaseModelDescriptor
    {
        [field: SerializeField, Tooltip("Brassicaceae - Крестоцветные \n Solanaceae - Паслёновые \n Asteraceae - Сложноцветные (астровые) \n Fabaceae - Бобовые \n Rosaceae - Розоцветные \n Poaceae - Злаковые \n Liliaceae - Лилейные \n Grossulariaceae - Крыжовниковые \n Ericaceae - Вересковые \n Hydrophyllaceae  - Водолистниковые \n Apiaceae  - Зонтичные \n Allioideae  - Луковые \n Cucurbitaceae  - Бахчевые \n Polygonaceae  - Гречишные \n Amaranthaceae  - Амарантовые \n")]
        public string Name { get; set; } = string.Empty;
        [field: SerializeField]
        public DiseaseType DiseaseType { get; set; }
        [field: SerializeField]
        [TableList]
        public List<PlantFamilyType> AffectedPlants { get; set; } = new();
        [field: SerializeField]
        public DiseaseInfectionModel InfectionModel { get; set; } = null!;
    }
}