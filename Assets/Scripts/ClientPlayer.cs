using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
  
public class ClientPlayer : MonoBehaviour
{
    [SerializeField] Texture2D colorPalette;
    private static List<Color> availableColors; //Colors not used from the available palette
    [SerializeField] TMP_Text playerNameText, playerNameTextBack;
    [SerializeField] SkinnedMeshRenderer smr;
    [SerializeField] protected Animator anim;
    [SerializeField] GameObject spineBone; //Used to change height of player

    protected string playerID, playerName;
    protected bool isLocal = false; //Is this the player being controlled by device?
    protected Color playerColor = Color.white;
    protected int headType;
    protected float height; //Between -0.2f anf 2.0f

    protected Vector3 movement;
    protected Quaternion lookRotation;
    protected float startingSpeed = 5, speed;
    protected bool canMove = true;

    public ClientManagerWeb clientManagerWeb = null; //Used to communicate with host


    protected PlayerInput playerInput;

    public string PlayerID { get => playerID; set => playerID = value; }
    public string PlayerName { get => playerName; set { playerNameText.text = playerNameTextBack.text = playerName = value; } }
    public Color PlayerColor { get => playerColor; set{ playerColor = value; ChangeColor(value); } }
    public int PlayerHeadType { get => headType; set{ headType = value; if (headType > -1) { smr.SetBlendShapeWeight(value, 100); } } }
    public float PlayerHeight { get => height; set{ height = value; Vector3 pos = spineBone.transform.localPosition; pos.y += height; spineBone.transform.localPosition = pos;} }

    public bool IsLocal { get => isLocal; set => isLocal = value; }
    public bool CanMove { get => canMove; set => canMove = value; }

    protected virtual void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    protected virtual void Awake()
    {
        //Instantiate list of available colors from palette
        if(availableColors == null)
        {
            availableColors = new List<Color>();
            for (int i = 0; i < colorPalette.width; i++)
            {
                availableColors.Add(colorPalette.GetPixel(i, 0));
            }
        }

        //smr only uses shared mesh. Have to instantiate individual mesh so they can have unique vertex colors
        Mesh m = smr.sharedMesh;
        Mesh m2 = Instantiate(m);
        smr.sharedMesh = m2;

        speed = startingSpeed;

        playerInput = GetComponent<PlayerInput>();
    }
    
    protected virtual void Update()
    {
        if (isLocal) //Only read values from analog stick, and emit movement if being done from local device
        {
            Vector2 input = playerInput.actions["Move"].ReadValue<Vector2>();
            Move((int)(input.x * 100), (int)(input.y * 100));

            clientManagerWeb.Manager.Socket.Emit("input", input.x, input.y);
        }

        transform.Translate(movement * Time.deltaTime);
        anim.transform.rotation = Quaternion.RotateTowards(lookRotation, transform.rotation, Time.deltaTime);
    }

    public void InitialCustomize()
    {
        //Color
        if (playerColor == Color.white)
        {
            Color newCol = availableColors[Random.Range(0, availableColors.Count)];
            ChangeColor(newCol);
        }

        //Head shapes
        headType = Random.Range(-1, smr.sharedMesh.blendShapeCount);
        if (headType > -1) //if -1, keep base head shape
        {
            smr.SetBlendShapeWeight(headType, 100);
        }

        //Height
        Vector3 pos = spineBone.transform.localPosition;
        height = Random.Range(-0.2f, 0.75f);
        pos.y += height;
        spineBone.transform.localPosition = pos;
    }

    private void ChangeColor(Color col)
    {
        Mesh mesh = smr.sharedMesh;
        Vector3[] vertices = mesh.vertices;

        Color[] colors = new Color[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            colors[i] = col;
        }
        availableColors.Remove(col);
        mesh.colors = colors;
        playerColor = col;
    }

    public void Move(int x, int y)
    {
        if (canMove)
        {
            movement = new Vector3(x * speed, 0, y * speed);

            //Magnitude of movement for animations
            float val = Mathf.Abs(new Vector2(x, y).magnitude);
            anim.SetFloat("Speed", val);

            //Update rotation
            Vector3 lookDirection = new Vector3(x, 0, y);
            if (lookDirection != Vector3.zero)
            {
                lookRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
                Debug.Log("Look Rotation: " + lookRotation);
            }
        }
        else
        {
            movement = Vector3.zero;
            anim.SetFloat("Speed", 0);
        }
    }

    //Default action, dance of course
    public virtual void Action()
    {
        int dance = Random.Range(1, 3);
        anim.SetTrigger("Dance" + dance);
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("COLLIDING WITH: " + collision.gameObject.name + " WITH TAG " + collision.gameObject.tag);
    }
}