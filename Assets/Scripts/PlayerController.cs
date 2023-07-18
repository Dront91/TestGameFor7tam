using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System;
using System.Collections;

public class PlayerController : MonoBehaviour, IPunObservable
{
    public Action OnDeath;
    public PhotonView _photonView;
    [SerializeField] private Text _nickname;
    public Text Nickname => _nickname;
    [SerializeField] private SpriteRenderer _sprite;
    [SerializeField] private int _maxHealth;
    [SerializeField] private float _attackRate;
    [SerializeField] private Image _healthBarImage;
    [SerializeField] private Image _coinBarImage;
    [SerializeField] private Text _healthBarText;
    [SerializeField] private Text _coinBarText;
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private GameObject _visualModel;
    public GameObject ProjectilePrefab => _projectilePrefab;
    private int _currentHealth;
    private float _timer;
    private int _moneyCount = 0;
    public int MoneyCount => _moneyCount;
    private bool _isControlActive;
    private float r;
    private float g;
    private float b;
    private float _doubleTapThreshold = 0.3f;
    private int _tapCount;
    private float _rotateSpeed = 2f;
    private void Start()
    {
        _photonView = GetComponent<PhotonView>();
        _nickname.text = _photonView.Owner.NickName;
        SetStartColor();
        _isControlActive = true;
        _currentHealth = _maxHealth;
        GameLevelController.Instance.AddPlayerToList(this);
        UpdateHealthUI();
        UpdateCoinUI();
    }
    void Update()
    {
        if(_photonView.IsMine && _isControlActive)
        {
            _timer += Time.deltaTime;
            float speed = 3f * Time.deltaTime;
            Vector3 translation = VirtualJoystick.Instance.Value * speed;
            transform.Translate(translation);
            RotateIcon();
            if (Input.touchCount == 1 )
            {
                if (Input.GetTouch(0).phase == TouchPhase.Ended )
                {
                    _tapCount++;
                    StartCoroutine(SingleOrDoubleTap());
                }
            }
            if(Input.GetMouseButtonDown(1))
            {
                Attack();
            }
        }
    }
    public void AddCoin()
    {
        _moneyCount++;
        UpdateCoinUI();
    }
    public void DamageResive(int damage)
    {
        _photonView.RPC("RPC_TakeDamage", RpcTarget.All, damage);
    }
    [PunRPC]
    void RPC_TakeDamage(int damage)
    {
        if (!_photonView.IsMine) { return; }
        _currentHealth -= damage;
        UpdateHealthUI();
        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            Death();
        }
    }
    private void Attack()
    {
        if(_timer > _attackRate)
        {
            var pos = transform.position + VirtualJoystick.Instance.FireDirection.normalized;
            var p = PhotonNetwork.Instantiate(_projectilePrefab.name, pos, Quaternion.identity);
            p.GetComponent<Projectile>().SetSettings(VirtualJoystick.Instance.FireDirection, this);
            _timer = 0;
        }
    }
    private void UpdateHealthUI()
    {
        _healthBarImage.fillAmount = (float)(((float)_currentHealth * 100 / (float)_maxHealth) / 100);
        _healthBarText.text = _currentHealth + "/" + _maxHealth;
    }
    private void UpdateCoinUI()
    {
        _coinBarImage.fillAmount = (float)_moneyCount / (float)GameLevelController.Instance.MaxCoinCount;
        _coinBarText.text = _moneyCount + "/" + GameLevelController.Instance.MaxCoinCount;
    }
    public void SwitchControl(bool value)
    {
        _isControlActive = value;
    }
    private void Death()
    {
        SwitchControl(false);
        _photonView.RPC("RPC_PlayerDeath", RpcTarget.All);
        gameObject.SetActive(false);
    }
    [PunRPC]
    void RPC_PlayerDeath()
    {
        OnDeath?.Invoke();
    }
    private void SetStartColor()
    {
        r = UnityEngine.Random.Range(0.1f, 1f);
        g = UnityEngine.Random.Range(0.1f, 1f);
        b = UnityEngine.Random.Range(0.1f, 1f);
        _sprite.color = new Color(r, g, b);
    }
    private void RotateIcon()
    {
        if (VirtualJoystick.Instance.FireDirection != Vector3.zero)
        {
            Quaternion q;
            q = Quaternion.LookRotation(VirtualJoystick.Instance.FireDirection, _visualModel.transform.up);
            _visualModel.transform.rotation = Quaternion.Lerp(_visualModel.transform.rotation, q, _rotateSpeed * Time.deltaTime);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(r);
            stream.SendNext(g);
            stream.SendNext(b);
            stream.SendNext(_currentHealth);
            stream.SendNext(_moneyCount);
        }
        else
        {
            r = (float)stream.ReceiveNext();
            g = (float)stream.ReceiveNext();
            b = (float)stream.ReceiveNext();
            _currentHealth = (int)stream.ReceiveNext();
            _moneyCount = (int)stream.ReceiveNext();
            UpdateCoinUI();
            UpdateHealthUI();
            _sprite.color = new Color(r, g, b);
        }
    }
    IEnumerator SingleOrDoubleTap()
    {
        yield return new WaitForSeconds(_doubleTapThreshold);

        if (_tapCount == 1)
        {
            Debug.Log("SingleTap");
            _tapCount = 0;
        }
        else if (_tapCount == 2)
        {
            Attack();
            _tapCount = 0;
        }

    }
}
