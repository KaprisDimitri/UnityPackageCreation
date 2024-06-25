using System;

public static class Utility_GenericSystem
{

    /// <summary>
    /// Use to know if is child of a generic type
    /// </summary>
    /// <param name="generic">Generic type exemple: A<> </param>
    /// <param name="toCheck"></param>
    /// <returns></returns>
    public static bool IsSubclassOfGeneric(Type generic, Type toCheck)
    {
        while (toCheck != null && toCheck != typeof(object))
        {
            // GetGenericTypeDefinition() => Entry: SO_Input_Base<float> Exit: SO_Input_Base<>
            var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
            if (generic == cur)
            {
                return true;
            }
            //Use to get the Type of the parent of the actual Type
            toCheck = toCheck.BaseType;
        }
        return false;
    }

    public static Type GetBaseGenericType(Type type, Type generic)
    {
        while (type != null && type != typeof(object))
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == generic)
            {
                return type;
            }
            type = type.BaseType;
        }
        return null;
    }
}
