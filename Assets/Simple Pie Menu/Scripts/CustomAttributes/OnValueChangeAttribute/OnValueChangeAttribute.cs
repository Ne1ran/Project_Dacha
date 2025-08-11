using UnityEngine;

namespace Simple_Pie_Menu.Scripts.CustomAttributes.OnValueChangeAttribute
{
    public class OnValueChangeAttribute : PropertyAttribute
    {
        public string methodName;

        public OnValueChangeAttribute(string methodName)
        {
            this.methodName = methodName;
        }
    }
}
