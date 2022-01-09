using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Photon.Pun;

namespace Cox.PlayerControls{
/// <summary> Receives inputs from either the player's controller and translates them into actions
/// in the Cox.PlayerControls.SpacecraftController.
///</summary>
public class InputHandler : MonoBehaviourPunCallbacks, ControlInputActions.IFlightActions
{
    float thrustInput, yawInput, brakeInput;
    int targetMode = 0;
    bool gunInput = false, missileInput = false;
    bool isMouse = false;

        
    SpacecraftController spacecraft;
    ControlInputActions _controls;
    Vector2 torqueInput, cursorInput;

    VisualElement root;

    public override void OnEnable() {
        root = FindObjectOfType<UIDocument>().rootVisualElement;
        
        cursorInput = new Vector2 (Screen.width / 2, Screen.height / 2);
        spacecraft = GetComponentInChildren<SpacecraftController>();
        _controls = new ControlInputActions();
        _controls.Flight.SetCallbacks(this);
        _controls.Flight.Enable();
        UnityEngine.Cursor.visible = false;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    }
    public override void OnDisable() {
        _controls.Flight.Disable();
    }
    //Update, Sends input data for rigidbody, camera rotation, and gun use.
    private void FixedUpdate() {
        if(photonView.IsMine)
        if(brakeInput > 0){
            spacecraft.BrakeControl();
        }
        if(thrustInput > 0){
            spacecraft.ThrustControl();
        }
        if(gunInput){
            spacecraft.GunControl(gunInput);
        }
        if(torqueInput != Vector2.zero || yawInput != 0){
            spacecraft.TorqueControl(torqueInput, yawInput);    
        }
        
        spacecraft.RotateCamera(cursorInput, isMouse);
    }

    //Inputs
    public void OnMenuButton(InputAction.CallbackContext pressed){
        root.Q<GameUIManager>().ToggleMenu();
    }
    public void OnTabMenuButton(InputAction.CallbackContext pressed){
        root.Q<GameUIManager>().ToggleTab();
    }
    public void OnBrake(InputAction.CallbackContext value){
        brakeInput = value.ReadValue<float>();
    }
    public void OnThrust(InputAction.CallbackContext value){
        thrustInput = value.ReadValue<float>();
        
    }
    public void OnTorque(InputAction.CallbackContext value){
        torqueInput = value.ReadValue<Vector2>();
    }
    public void OnYaw(InputAction.CallbackContext value){
        yawInput = value.ReadValue<float>();
    }
    public void OnGunFire(InputAction.CallbackContext pressed){
        gunInput = pressed.ReadValueAsButton();
    }
    public void OnCameraChange(InputAction.CallbackContext context){
        spacecraft.CameraChange();
    }
    public void OnCycleTargets(InputAction.CallbackContext pressed){
        if(pressed.ReadValueAsButton()){
            spacecraft.CycleTargets();
        }
        
    }
    public void OnMissileButton(InputAction.CallbackContext pressed){
        spacecraft.MissileLaunch();
    }
    public void OnPrimaryAbility(InputAction.CallbackContext pressed){
        if(pressed.ReadValueAsButton())
        spacecraft.PrimaryAbility();
    }
    public void OnSecondaryAbility(InputAction.CallbackContext pressed){
        if(pressed.ReadValueAsButton())
        spacecraft.SecondaryAbility();
    }
    public void OnAceAbility(InputAction.CallbackContext pressed){
        if(pressed.ReadValueAsButton())
        spacecraft.AceAbility();
    }
    public void OnCameraStick(InputAction.CallbackContext stickInput){
        cursorInput = stickInput.ReadValue<Vector2>();
        isMouse = false;
    }
    public void OnCameraMouse(InputAction.CallbackContext deltaInput){
        StopAllCoroutines();
        cursorInput += deltaInput.ReadValue<Vector2>();
        cursorInput = new Vector2(Mathf.Clamp(cursorInput.x, -120, 120), Mathf.Clamp(cursorInput.y, -70, 70));
        isMouse = true;
        StartCoroutine(ResetMouseInput());
    }

    public void OnTargetModeAdd(InputAction.CallbackContext pressed)
    {
        if(photonView.IsMine)
        if(pressed.ReadValueAsButton())targetMode += 1;
        if(targetMode > 2)targetMode = 0;
        spacecraft.ChangeTargetMode(targetMode);
    }

    public void OnTargetModeSub(InputAction.CallbackContext pressed)
    {
        if(photonView.IsMine)
        if(pressed.ReadValueAsButton())targetMode -= 1;
        if(targetMode > 2)targetMode = 0;
        if(targetMode < 0)targetMode = 2;
        spacecraft.ChangeTargetMode(targetMode);
    }

    public void OnCameraTargetLock(InputAction.CallbackContext context)
    {
        if(photonView.IsMine)
        spacecraft.CameraLockTarget();
    }
    private IEnumerator ResetMouseInput(){
        yield return new WaitForSecondsRealtime(3);
        cursorInput = Vector2.zero;
    }
}
}
