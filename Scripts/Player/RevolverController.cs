using UnityEngine;
using System.Collections;

public class RevolverController : MonoBehaviour 
{
	[SerializeField]
	private WeaponManager _weaponManager;

	[SerializeField]
	private AudioClip[] _hammers;
	[SerializeField]
	private AudioClip[] _shots;

	[SerializeField]
	private AudioSource _hammerSource;
	[SerializeField]
	private AudioSource _shotSource;

	[SerializeField]
	private float _revRotSpeed;			// revolver rotation speed
	[SerializeField]
	private Transform _cylTr;               // ref to cylinder of revolver

	private bool rotatingRev = false;
	private bool rotatingCyl = false;
	private float endAngle = 60F;		// rotated angle

	void Start () 
	{
		_cylTr = _cylTr.transform;
		rotatingCyl = true;
	}

	void Update () 
	{
		if (rotatingRev)
			transform.Rotate (Vector3.up * _revRotSpeed * Time.deltaTime);

		if (rotatingCyl) 
		{
			RotateCyl ();
		}
	}

	public void FlipRevState () 
	{
		rotatingRev = !rotatingRev;
	}

	public void RotateCyl () 
	{
		if (endAngle == 360F && _cylTr.localRotation.eulerAngles.y < 60F) 
		{
			endAngle = 0F;
		}

		if (_cylTr.localRotation.eulerAngles.y < endAngle ) 
		{
			rotatingCyl = true;
			Quaternion target = Quaternion.Euler (0, endAngle, 0); 
			_cylTr.localRotation = Quaternion.RotateTowards (_cylTr.localRotation, target, Time.deltaTime * 100F);
		} 
		else 
		{
			rotatingCyl = false;
			endAngle += 60F;
		}
	}

	public void Shoot()
    {
		_shotSource.clip = _shots[Random.Range(0, _shots.Length)];
		_shotSource.Play();

		_weaponManager.RegisterHittingHitbox();
	}

	public void Hammer()
    {
		_hammerSource.clip = _hammers[Random.Range(0, _hammers.Length)];
		_hammerSource.Play();
	}
}
