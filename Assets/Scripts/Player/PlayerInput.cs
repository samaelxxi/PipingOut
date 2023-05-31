using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum CameraRotationCommand { Top, Left, Right, None }

public class PlayerInput
{
    protected readonly PlayerCharacterInputs _lastInputs = new();


    protected bool _canControl = true;

    public void SetCanControl(bool canControl)
    {
        // Debug.Log($"SetCanControl {canControl}");
        _canControl = canControl;
    }

    public virtual bool GetRestartCommand()
    {
        if (Input.GetKeyDown(KeyCode.T))
            return true;
        return false;
    }

    public virtual bool GetEscCommand()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            return true;
        return false;
    }

    public virtual PlayerCharacterInputs GetMoveAxis(bool isTopView, int currentRotation)
    {
        if (!_canControl)
        {
            _lastInputs.MoveAxisForward = 0;
            _lastInputs.MoveAxisRight = 0;
            _lastInputs.JumpDown = false;
            return _lastInputs;
        }

        Vector2 input = new(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        _lastInputs.MoveAxisForward = CalcForwardAxis(input, isTopView, currentRotation);
        _lastInputs.MoveAxisRight = CalcRightAxis(input, isTopView, currentRotation);
        _lastInputs.JumpDown = IsJumpEnabled(input, isTopView);
        return _lastInputs;
    }

    public PlayerCharacterInputs GetStopMoveAxis()
    {
        _lastInputs.MoveAxisForward = 0;
        _lastInputs.MoveAxisRight = 0;
        _lastInputs.JumpDown = false;
        return _lastInputs;
    }

    float CalcForwardAxis(Vector2 input, bool isTopView, int currentRotation)
    {
        float forward;
        if (isTopView)
        {
            forward = currentRotation % 2 == 0 ? input.y : input.x;
            forward = (currentRotation == 0 || currentRotation == 3) ? forward : -forward;
        }
        else
        {
            forward = currentRotation % 2 == 1 ? input.x : 0;
            forward = currentRotation > 2 ? forward : -forward;
        }
        return forward;
    }

    float CalcRightAxis(Vector2 input, bool isTopView, int currentRotation)
    {
        float right;
        if (isTopView)
        {
            right = currentRotation % 2 == 0 ? input.x : input.y;
            right = currentRotation > 1 ? -right : right;
        }
        else
        {
            right = currentRotation % 2 == 0 ? input.x : 0;
            right = currentRotation > 1 ? -right : right;
        }
        return right;
    }

    protected virtual bool IsJumpEnabled(Vector2 input, bool isTopView)
    {
        return isTopView ? false : input.y > 0.01f;
    }

    public virtual CameraRotationCommand GetCameraRotationCommand()
    {
        if (!_canControl)
            return CameraRotationCommand.None;

        if (Input.GetKeyDown(KeyCode.Q))
            return CameraRotationCommand.Left;
        if (Input.GetKeyDown(KeyCode.E))
            return CameraRotationCommand.Right;
        if (Input.GetKeyDown(KeyCode.Space))
            return CameraRotationCommand.Top;
        return CameraRotationCommand.None;
    }
}
