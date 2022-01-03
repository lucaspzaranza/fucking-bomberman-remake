// GENERATED AUTOMATICALLY FROM 'Assets/Input/BombermanInput.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @BombermanInput : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @BombermanInput()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""BombermanInput"",
    ""maps"": [
        {
            ""name"": ""PlayerControls"",
            ""id"": ""e046b89a-aaac-46a6-b06d-59e6ce77def6"",
            ""actions"": [
                {
                    ""name"": ""WASD Moves"",
                    ""type"": ""Button"",
                    ""id"": ""4f4c0122-b380-4406-9c49-a5d834b42020"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Bomb"",
                    ""type"": ""Button"",
                    ""id"": ""c53a1bc5-bcb9-48a5-bf3b-2ce051bd1e8a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Force"",
                    ""type"": ""Button"",
                    ""id"": ""20031ad0-48e8-41e1-8e85-15989181210b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Push"",
                    ""type"": ""Button"",
                    ""id"": ""733a3a0d-b5d4-4de3-8e59-71d840d34e08"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD Axis"",
                    ""id"": ""d3545d91-76b6-40e0-b2fa-aeb8a43632d7"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""WASD Moves"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""3e646700-90cb-4152-8d1a-e644d33dacc1"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""WASD Moves"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""def07728-31ea-452a-b5f4-bf6cb23fb922"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""WASD Moves"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""26a74c90-7993-49ff-bdd0-9f39d9a2d90f"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""WASD Moves"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""d38761e3-cb15-4250-962b-ede7bd205d99"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""WASD Moves"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Arrows "",
                    ""id"": ""7a6bc0f7-07ae-4ad6-9c07-f17933bf3df2"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""WASD Moves"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""58be7417-f3df-4e1c-b3ad-17db084b57c1"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""WASD Moves"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""81f3dad7-825c-4b73-b8b3-22b1220b7552"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""WASD Moves"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""a56db08b-f0ad-4403-aa35-54106b942d3c"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""WASD Moves"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""4732b6d8-01f3-4704-a09e-511512d40af8"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""WASD Moves"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""17caad1e-c047-47ec-b002-3453e51d6fca"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Bomb"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ab65dab5-4873-42ae-ac3a-e4e7929580a0"",
                    ""path"": ""<Keyboard>/c"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Force"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""218a086d-c4e3-432c-8d96-82e0b2cd5c2d"",
                    ""path"": ""<Keyboard>/ctrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Push"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // PlayerControls
        m_PlayerControls = asset.FindActionMap("PlayerControls", throwIfNotFound: true);
        m_PlayerControls_WASDMoves = m_PlayerControls.FindAction("WASD Moves", throwIfNotFound: true);
        m_PlayerControls_Bomb = m_PlayerControls.FindAction("Bomb", throwIfNotFound: true);
        m_PlayerControls_Force = m_PlayerControls.FindAction("Force", throwIfNotFound: true);
        m_PlayerControls_Push = m_PlayerControls.FindAction("Push", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // PlayerControls
    private readonly InputActionMap m_PlayerControls;
    private IPlayerControlsActions m_PlayerControlsActionsCallbackInterface;
    private readonly InputAction m_PlayerControls_WASDMoves;
    private readonly InputAction m_PlayerControls_Bomb;
    private readonly InputAction m_PlayerControls_Force;
    private readonly InputAction m_PlayerControls_Push;
    public struct PlayerControlsActions
    {
        private @BombermanInput m_Wrapper;
        public PlayerControlsActions(@BombermanInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @WASDMoves => m_Wrapper.m_PlayerControls_WASDMoves;
        public InputAction @Bomb => m_Wrapper.m_PlayerControls_Bomb;
        public InputAction @Force => m_Wrapper.m_PlayerControls_Force;
        public InputAction @Push => m_Wrapper.m_PlayerControls_Push;
        public InputActionMap Get() { return m_Wrapper.m_PlayerControls; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerControlsActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerControlsActions instance)
        {
            if (m_Wrapper.m_PlayerControlsActionsCallbackInterface != null)
            {
                @WASDMoves.started -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnWASDMoves;
                @WASDMoves.performed -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnWASDMoves;
                @WASDMoves.canceled -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnWASDMoves;
                @Bomb.started -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnBomb;
                @Bomb.performed -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnBomb;
                @Bomb.canceled -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnBomb;
                @Force.started -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnForce;
                @Force.performed -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnForce;
                @Force.canceled -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnForce;
                @Push.started -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnPush;
                @Push.performed -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnPush;
                @Push.canceled -= m_Wrapper.m_PlayerControlsActionsCallbackInterface.OnPush;
            }
            m_Wrapper.m_PlayerControlsActionsCallbackInterface = instance;
            if (instance != null)
            {
                @WASDMoves.started += instance.OnWASDMoves;
                @WASDMoves.performed += instance.OnWASDMoves;
                @WASDMoves.canceled += instance.OnWASDMoves;
                @Bomb.started += instance.OnBomb;
                @Bomb.performed += instance.OnBomb;
                @Bomb.canceled += instance.OnBomb;
                @Force.started += instance.OnForce;
                @Force.performed += instance.OnForce;
                @Force.canceled += instance.OnForce;
                @Push.started += instance.OnPush;
                @Push.performed += instance.OnPush;
                @Push.canceled += instance.OnPush;
            }
        }
    }
    public PlayerControlsActions @PlayerControls => new PlayerControlsActions(this);
    public interface IPlayerControlsActions
    {
        void OnWASDMoves(InputAction.CallbackContext context);
        void OnBomb(InputAction.CallbackContext context);
        void OnForce(InputAction.CallbackContext context);
        void OnPush(InputAction.CallbackContext context);
    }
}
