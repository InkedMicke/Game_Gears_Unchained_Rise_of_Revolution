using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(GroundDetection))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(PlayerCustomMovment))]
public class CharacterControllerV2 : MonoBehaviour
{
    [SerializeField, HideInInspector]Rigidbody _rb;
    [SerializeField, HideInInspector]GroundDetection _groundDetect;
    [SerializeField, HideInInspector]PlayerCustomMovment _playerCustomMovment;
    private PlayerInputActions _inputActions;

    private void Awake()
    {
        _inputActions = new PlayerInputActions();
        _inputActions.Enable();
    }

    private void OnValidate()
    {
        _rb = GetComponent<Rigidbody>();
        _groundDetect = GetComponent<GroundDetection>();
        _playerCustomMovment = GetComponent<PlayerCustomMovment>();
    }

    void FixedUpdate()
    {
        PlayerHorizontalMovment(CameraOritentedMovment(GetInput()));
    }
    
    

    Vector2 GetInput()// TODO Replace with new input system
    {
        Vector2 moveInput;
        moveInput = _inputActions.PlayerPC.MovementKeyboard.ReadValue<Vector2>();

        moveInput.Normalize();
        return moveInput;
    }

    void PlayerHorizontalMovment(Vector2 input)
    {
        float vVel = Vector3.Dot(_rb.velocity, _groundDetect.GroundNormal);

        Vector2 currentVel2d = Get2dOrientation(Vector3.ProjectOnPlane(_rb.velocity, _groundDetect.GroundNormal), _groundDetect.GroundNormal);
        

        currentVel2d = _playerCustomMovment.Movment(currentVel2d, input);


        Debug.DrawRay(transform.position + transform.up*0.8f, new Vector3(currentVel2d.x,0f,currentVel2d.y), Color.green);
        Vector3 outputHorizontal = MutiplyByPlane(currentVel2d, _groundDetect.GroundNormal);
        Debug.DrawRay(transform.position + transform.up*0.8f, outputHorizontal, Color.blue);
        _rb.velocity = outputHorizontal + (_groundDetect.GroundNormal * vVel);
    }

    
    Vector2 CameraOritentedMovment(Vector2 input)
    {
        Transform cameraTransform = Camera.main.transform;
        Vector3 camRight = cameraTransform.right;
        Vector3 camForward = cameraTransform.forward;

        camForward = input.y* Vector3.ProjectOnPlane(camForward, Vector3.up).normalized;
        camRight = input.x * Vector3.ProjectOnPlane(camRight, Vector3.up).normalized;

        Vector3 camForwardAxis =  Vector3.ProjectOnPlane(Vector3.forward, _groundDetect.GroundNormal).normalized;
        Vector3 camRightAxis =  Vector3.ProjectOnPlane(Vector3.right, _groundDetect.GroundNormal).normalized;

        Vector2 output;

        output.x = (Vector3.Dot(camRight + camForward, camRightAxis));
        output.y = (Vector3.Dot(camForward + camRight, camForwardAxis));

        Debug.DrawRay(transform.position + (transform.up * 2.5f), new Vector3(output.x, 0f, output.y), Color.cyan);

        return output;
    }
    /// <summary>
    /// change a vector from a 3d vector to a 2d vector 
    /// </summary>
    Vector2 Get2dOrientation(Vector3 value, Vector3 normal) // value is any vector, normal always comes in normalized
    {
        // project the value so we are on the plane defined by the normal
        Vector3 modifiedValue = Vector3.ProjectOnPlane(value, normal).normalized;
        // get the right direction 
        Vector3 right = Vector3.Cross(normal, modifiedValue).normalized;
        // get the forward direction translated to the xz plane
        Vector3 inputDir = Vector3.Cross(right, Vector3.up).normalized;
        //add the magnitude back to make it as long as at the start
        Vector2 output = new Vector2(inputDir.x , inputDir.z).normalized * value.magnitude;
        
        return output;
    }

    /// <summary>
    /// transforms a 2d plane into a 3d one based on a normal  
    /// </summary>
    Vector3 MutiplyByPlane(Vector2 plane, Vector3 planeNormal)
    {
        Vector3 planeDir = new Vector3(plane.x, 0f, plane.y);
        Vector3 translatedRight = Vector3.Cross(planeNormal, planeDir.normalized).normalized;
        Vector3 translatedForward = Vector3.Cross(translatedRight, planeNormal).normalized;
        Vector3 output = translatedForward * plane.magnitude;
        return output;
    }

    float sphereRadius;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;

        Vector3 playerPos = transform.position + (transform.up * 2.5f);

        Gizmos.DrawWireSphere(playerPos, sphereRadius);

        /* temporal testing
        if(Application.isPlaying)
        {
            Gizmos.DrawWireSphere(playerPos, testingVector.magnitude);

            Gizmos.color = Color.green;
            Gizmos.DrawRay(playerPos, testingVector);
            Gizmos.color = Color.yellow;
            Vector2 transformedDir = Get2dOrientation(testingVector, _groundDetect.GroundNormal);
            Gizmos.DrawRay(playerPos, new Vector3(transformedDir.x, 0f, transformedDir.y));
            Gizmos.color = Color.red;
            Gizmos.DrawRay(playerPos, MutiplyByPlane(transformedDir, _groundDetect.GroundNormal));
        }
        */
    }
}
