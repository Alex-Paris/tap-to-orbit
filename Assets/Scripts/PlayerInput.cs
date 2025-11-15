using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;

public class PlayerInput : MonoBehaviour
{
    private InputUser _inputUser;
    private InputActionAsset _localActionAsset;
    
    public InputUser InputUser => _inputUser;
    public InputActionAsset ActionAsset => _localActionAsset;
    
    private void Awake()
    {
        _localActionAsset = InputActionAsset.FromJson(InputSystem.actions.ToJson());

        _inputUser = InputUser.CreateUserWithoutPairedDevices(); 
        _inputUser.AssociateActionsWithUser(_localActionAsset);

        PairAllCurrentDevices();
        // _localActionAsset.Enable();

        // Auto pair new device
        InputUser.listenForUnpairedDeviceActivity = 1;
        InputUser.onUnpairedDeviceUsed += OnUnpairedDeviceUsed;
    }
    
    private void OnDestroy()
    {
        InputUser.onUnpairedDeviceUsed -= OnUnpairedDeviceUsed;
        InputUser.listenForUnpairedDeviceActivity = 0;
        if (_localActionAsset != null) _localActionAsset.Disable();
    }
    
    public void ActivateScheme(string name)
    {
        if (string.IsNullOrEmpty(name)) return;
        _inputUser.ActivateControlScheme(name);
    }
    
    public void PairAllCurrentDevices()
    {
        // Teclado/Mouse
        if (Keyboard.current != null)  InputUser.PerformPairingWithDevice(Keyboard.current,  _inputUser);
        if (Mouse.current != null)     InputUser.PerformPairingWithDevice(Mouse.current,     _inputUser);

        // Touch
        if (Touchscreen.current != null) InputUser.PerformPairingWithDevice(Touchscreen.current, _inputUser);

        // Gamepads existentes
        foreach (var gp in Gamepad.all)
            InputUser.PerformPairingWithDevice(gp, _inputUser);
    }
    
    private void OnUnpairedDeviceUsed(InputControl control, InputEventPtr evt)
    {
        // Ex: um novo gamepad conectado durante o jogo
        if (!_inputUser.valid) return;
        InputUser.PerformPairingWithDevice(control.device, _inputUser);
    }
}
