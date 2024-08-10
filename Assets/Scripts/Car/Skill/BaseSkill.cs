using UnityEngine;

namespace BumperCarProject.Car.Skill
{
    public abstract class BaseSkill : MonoBehaviour
    {
        [field: SerializeField]
        public float SkillCoolTime { get; set; }
        
        // Add Something...
    }
}
