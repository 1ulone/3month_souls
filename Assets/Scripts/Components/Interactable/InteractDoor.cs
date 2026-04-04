using UnityEngine;

public class InteractDoor : MonoBehaviour
{
    [SerializeField] private bool isHorizontal;
    private BoxCollider boxCollider;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    public void EnterTransition(PlayerController con, CharacterController rb, InputController input)
    {
        FadeTransitionUI.instances.StartTransition(
            ()=> {
                rb.Move(Vector3.zero);
                input.DisableInput();

                rb.enabled = false;
            }, 
            ()=> {
                Vector3 newPos = new Vector3(
                    isHorizontal ? (con.ActualMesh.forward.x > 0 ? boxCollider.bounds.max.x : boxCollider.bounds.min.x) : con.transform.position.x,
                    con.transform.position.y,
                    isHorizontal ? con.transform.position.z : (con.ActualMesh.forward.z > 0 ? boxCollider.bounds.max.z : boxCollider.bounds.min.z)
                );

                con.transform.position = newPos + (con.ActualMesh.forward * 2.5f);

                Collider[] checkBounding = Physics.OverlapBox(con.transform.position, Vector3.one, Quaternion.identity, con.roomBoundLayer);
                if (checkBounding.Length > 0)
                {
                    if (checkBounding[0].TryGetComponent<RoomTriggerComponent>(out RoomTriggerComponent rtc))
                        rtc.TriggerBoundingBox();
                }
            },
            ()=> {
                rb.enabled = true;

                rb.Move(Vector3.zero);
                input.EnableInput();
            }
        );
    }
}
