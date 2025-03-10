using System;
using static GameActionsMapsNames;

public static class GameActionsMapsNames {

    //DO NOT REMOVE ANY THING IN THIS ENUM GENERATED BY InputManagerEditor
    public enum GameActionsEnum 
    {
        GameActionMap_MovingVector,
        ExempleActionMap_Moving,
        //GameActionInsertLine
    }

    //TO DO: MAKE A value 0 like none (change the for value in  GetEnumActionMapByString and GetEnumActionByString 0 to 1)
    //DO NOT REMOVE ANY THING IN THIS ENUM GENERATED BY InputManagerEditor
    public enum GameActionsMapsEnum
    {
        ExempleActionMap,
        GameActionMap,
        //GameActionMapInsertLine
    }

    public static bool GetEnumActionByString (string value, out GameActionsEnum result)
    {
        for (int i = 0; i < Enum.GetValues(typeof(GameActionsEnum)).Length; i++)
        {
            if(value == ((GameActionsEnum)i).ToString())
            {
                result = (GameActionsEnum)i;
                return true;
            }
        }
        result = 0;
        return false;
    }

    public static bool GetEnumActionMapByString(string value, out GameActionsMapsEnum result)
    {
        for (int i = 0; i < Enum.GetValues(typeof(GameActionsMapsEnum)).Length; i++)
        {
            if (value == ((GameActionsMapsEnum)i).ToString())
            {
                result = (GameActionsMapsEnum)i;
                return true;
            }
        }
        result = 0;
        return false;
    }
}

/*
public enum GameActionsEnum { 


        FirstPersonActionMap_MovingAction,
        FirstPersonActionMap_RotateAction,
        FirstPersonActionMap_Sprint,

        InMenuActionMap_fesfesfe,
        InMenuActionMap_ValidAction,
        InMenuActionMap_SalutSalut,

        //GameActionInsertLine
    }

    public enum GameActionsMapsEnum
    {


        FirstPersonActionMap,

        InMenuActionMap,

        //GameActionMapInsertLine
    }

    public static bool GetEnumActionByString (string value, out GameActionsEnum result)
    {
        for (int i = 1; i < Enum.GetValues(typeof(GameActionsEnum)).Length; i++)
        {
            if(value == ((GameActionsEnum)i).ToString())
            {
                result = (GameActionsEnum)i;
                return true;
            }
        }
        result = 0;
        return false;
    }

    public static bool GetEnumActionMapByString(string value, out GameActionsMapsEnum result)
    {
        for (int i = 1; i < Enum.GetValues(typeof(GameActionsMapsEnum)).Length; i++)
        {
            if (value == ((GameActionsMapsEnum)i).ToString())
            {
                result = (GameActionsMapsEnum)i;
                return true;
            }
        }
        result = 0;
        return false;
    }
 */
