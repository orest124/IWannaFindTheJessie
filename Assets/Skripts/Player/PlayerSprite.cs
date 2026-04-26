using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class PlayerSprite
{
    [SerializeField] Movement mc;
    
    [Header("Imagine")] 
    [SerializeField] AnimationSprite spriteRenderUp;
    [SerializeField] AnimationSprite spriteRenderDown;
    [SerializeField] AnimationSprite spriteRenderLeft;
    [SerializeField] AnimationSprite spriteRenderRight;
    AnimationSprite activeSpriteRender;
    
    [SerializeField] GameObject lgt;
    [SerializeField] Tilemap DarkestZone;
    public void LStart()
    {
        activeSpriteRender = spriteRenderDown;
        ChengSprite(Vector3.down);
        
    }
    public bool RedyToMove() => activeSpriteRender.RedyToMove();
    public void Falling() => activeSpriteRender.Falling();
    public void StendUp() => activeSpriteRender.StendUp();
    public void ChengSprite(Vector3 dir)
    {
        if(!activeSpriteRender.RedyToMove()) return;
        Vector3 _dir = dir;
        if(_dir == Vector3.up )
            activeSpriteRender = spriteRenderUp;
        else spriteRenderUp.enabled = false;

        if (_dir == Vector3.down)
            activeSpriteRender = spriteRenderDown;
        else spriteRenderDown.enabled = false;

        if(_dir == Vector3.left || _dir == new Vector3(-1,1,0) || _dir == new Vector3(-1,-1,0)) 
        activeSpriteRender = spriteRenderLeft;
        else spriteRenderLeft.enabled = false;

        if(_dir == Vector3.right || _dir == new Vector3(1,-1,0) || _dir == new Vector3(1,1,0)) 
        activeSpriteRender = spriteRenderRight;
        else spriteRenderRight.enabled = false;

        activeSpriteRender.sp.sortingOrder = -Mathf.RoundToInt(mc.transform.position.y * 10);
        activeSpriteRender.enabled = true;
        activeSpriteRender.idle = _dir == Vector3.zero;
        
    }
    public void AnimSpd(bool _desh)
    {
        spriteRenderUp.animationTime = _desh? 0.06f: 0.1f;
        spriteRenderDown.animationTime = _desh? 0.06f: 0.1f;
        spriteRenderLeft.animationTime = _desh? 0.06f: 0.1f;
        spriteRenderRight.animationTime = _desh? 0.06f: 0.1f;
    }
    
    public bool inSnow;
    public void ShedowAudit()
    {
        if(mc.curentDore != mc.abuseDore)
        {
            Vector3Int cellPos = DarkestZone.WorldToCell(mc.transform.position);
            var tile = DarkestZone.GetTile(cellPos);

            if(tile != null) {lgt.SetActive(false);inSnow = true; }
            else {lgt.SetActive(true); inSnow = false; }
        }
    }

}