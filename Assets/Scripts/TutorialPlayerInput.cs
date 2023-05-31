using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tutorial
{
    public class TutorialPlayerInput : PlayerInput
    {
        bool _isJumpEnabled = false;
        bool _isRestartEnabled = false;
        bool _isVertRotationEnabled = false;
        bool _isHorRotationEnabled = false;

        bool _isFirstRestartClicked = false;
        public event Action OnFirstRestartClick;
        bool _isFirstVerticalRotationClicked = false;
        public event Action OnFirstVerticalRotationClick;
        bool _isFirstHorizontalRotationClicked = false;
        public event Action OnFirstHorizontalRotationClick;
        bool _isFirstJumpClicked = false;
        public event Action OnFirstJumpClick;

        public override bool GetRestartCommand()
        {
            if (!_isRestartEnabled)
                return false;
            if (Input.GetKeyDown(KeyCode.T))
            {
                if (!_isFirstRestartClicked)
                {
                    _isFirstRestartClicked = true;
                    OnFirstRestartClick?.Invoke();
                }
                return true;
            }
            return false;
        }

        protected override bool IsJumpEnabled(Vector2 input, bool isTopView)
        {
            if (_isJumpEnabled && input.y > 0.01f && !_isFirstJumpClicked)
            {
                _isFirstJumpClicked = true;
                OnFirstJumpClick?.Invoke();
            }
            return (isTopView || !_isJumpEnabled) ? false : input.y > 0.01f;
        }

        public override CameraRotationCommand GetCameraRotationCommand()
        {
            if (!_canControl)
                return CameraRotationCommand.None;

            if (Input.GetKeyDown(KeyCode.Q) && _isHorRotationEnabled)
            {
                if (!_isFirstHorizontalRotationClicked)
                {
                    _isFirstHorizontalRotationClicked = true;
                    OnFirstHorizontalRotationClick?.Invoke();
                }
                return CameraRotationCommand.Left;
            }
            if (Input.GetKeyDown(KeyCode.E) && _isHorRotationEnabled)
            {
                if (!_isFirstHorizontalRotationClicked)
                {
                    _isFirstHorizontalRotationClicked = true;
                    OnFirstHorizontalRotationClick?.Invoke();
                }
                return CameraRotationCommand.Right;
            }
            if (Input.GetKeyDown(KeyCode.Space) && _isVertRotationEnabled)
            {
                if (!_isFirstVerticalRotationClicked)
                {
                    _isFirstVerticalRotationClicked = true;
                    OnFirstVerticalRotationClick?.Invoke();
                }
                return CameraRotationCommand.Top;
            }
            return CameraRotationCommand.None;
        }

        public void EnableJump()
        {
            _isJumpEnabled = true;
        }

        public void EnableRestart()
        {
            _isRestartEnabled = true;
        }

        public void EnableVerticalRotation()
        {
            _isVertRotationEnabled = true;
        }

        public void EnableHorizontalRotation()
        {
            _isHorRotationEnabled = true;
        }
    }
}
