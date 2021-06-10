using UnityEngine;
using UnityEngine.EventSystems;

public class BtnFireScript : MonoBehaviour, IPointerDownHandler,IPointerUpHandler
{
    public MyPlayer player;
    bool firing = false;


    public void Update()
    {
        /*if (firing)
        {
            player.Fire();
        }*/
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        firing = true;
        player.Fire();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        firing = false;
        player.FireUp();
    }

    public void SetPlayer(MyPlayer _player)
    {
        player = _player;
    }

   


}
