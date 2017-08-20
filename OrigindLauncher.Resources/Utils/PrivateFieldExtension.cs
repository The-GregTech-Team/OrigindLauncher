using System.Reflection;

namespace OrigindLauncher.Resources.Utils
{
    public static class PrivateFieldExtension
    {
        public static T GetPrivateField<T>(this object instance, string fieldname)
        {
            const BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic;
            var type = instance.GetType();
            var field = type.GetField(fieldname, flag);
            return (T) field.GetValue(instance);
        }
        
    }
}