using System.Reflection;

namespace OrigindLauncher.Resources.Utils
{
    public static class PrivateHelper
    {
        public static T GetPrivateField<T>(this object instance, string fieldname)
        {
            const BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic;
            var type = instance.GetType();
            var field = type.GetField(fieldname, flag);
            return (T) field.GetValue(instance);
        }

        public static T GetPrivateProperty<T>(this object instance, string propertyname)
        {
            const BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic;
            var type = instance.GetType();
            var field = type.GetProperty(propertyname, flag);
            return (T) field.GetValue(instance, null);
        }
    }
}