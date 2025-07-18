using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;

namespace Core.Resources.Binding.Binding
{
    internal class BindingModel
    {
        protected string Name { get; }
        private MemberInfo MemberInfo { get; }

        protected BindingModel(string name, MemberInfo memberInfo)
        {
            Name = name;
            MemberInfo = memberInfo;
        }

        protected void SetField(MonoBehaviour component, object value)
        {
            FieldInfo info = MemberInfo as FieldInfo;
            if (info != null) {
                info.SetValue(component, value);
                return;
            }

            (MemberInfo as PropertyInfo)?.SetValue(component, value, null);
        }

        [CanBeNull]
        protected GameObject GetChildByName(GameObject gameObject, string name)
        {
            return gameObject.GetComponentsInChildren<Transform>(true)
                             .Where(c => c.gameObject != gameObject)
                             .FirstOrDefault(c => c.gameObject.name == name)
                             ?.gameObject;
        }
    }
}