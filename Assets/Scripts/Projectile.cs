using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public PhotonView _photonView;
    [SerializeField] private float _speed;
    [SerializeField] private int _damage;
    private float _lifeTime = 5f;
    private Vector3 _direction;
    private float _timer;
    private PlayerController _parent;
    private float _step;
    private void Start()
    {
        _photonView = GetComponent<PhotonView>();
    }
    private void Update()
    {
        var translation = _direction * _speed * Time.deltaTime;
        _step = translation.magnitude;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, _direction, _step);
        if(hit)
        {
            if(hit.transform.TryGetComponent(out PlayerController player))
            {
                if(player != _parent)
                {
                    player.DamageResive(_damage);
                    DestroyProjectile();
                }
            }
        }
        transform.Translate(translation);
        _timer += Time.deltaTime;
        if(_timer > _lifeTime)
        {
            DestroyProjectile();
        }
    }
    public void SetSettings(Vector3 direction, PlayerController parent)
    {
        _direction = direction.normalized;
        _parent = parent;
    }
    private void DestroyProjectile()
    {
        _photonView.RPC("RPC_DestroyProjectile", RpcTarget.All);
    }
    [PunRPC]
    void RPC_DestroyProjectile()
    {
        Destroy(gameObject);
    }
}
