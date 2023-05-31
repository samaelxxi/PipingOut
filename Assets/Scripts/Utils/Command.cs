using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum CommandOnTrigger { None, SetMovement, SetObjectActive, SetObjectNotActive, EnablePlatform, 
    DisablePlatform, ChangeStepAngle }
public enum CommandType { None, Int, Bool, GameObject }
public enum BoolCommand { None, SetMovement }
public enum ObjCommand { None, SetObjectActive, SetObjectNotActive, EnablePlatform, DisablePlatform }
public enum IntCommand { None, ChangeStepAngle }

public class Command
{
    public Command(CommandOnTrigger command, int argument, GameObject obj, bool b)
    {
        CommandType = command;
        Argument = argument;
        Obj = obj;
        BoolArg = b;
    }

    public CommandOnTrigger CommandType;
    public int Argument;
    public GameObject Obj;
    public bool BoolArg;
}
